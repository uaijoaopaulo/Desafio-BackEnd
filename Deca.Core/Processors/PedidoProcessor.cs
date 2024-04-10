using Desa.Core.Exceptions;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Desa.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors
{
    public class PedidoProcessor : IPedidoProcessor
    {
        protected readonly IPedidoRepository _pedidoRepository;
        protected readonly ILocacaoRepository _locacaoRepository;
        protected readonly INotificacaoPedidoRepository _notificacaoPedidoRepository;
        protected readonly IEntregadorRepository _entregadorRepository;
        private readonly ISqsRepository _sqsRepository;
        public PedidoProcessor(IPedidoRepository pedidoRepository, ILocacaoRepository locacaoRepository, INotificacaoPedidoRepository notificacaoPedidoRepository, ISqsRepository sqsRepository, IEntregadorRepository entregadorRepository)
        {
            _pedidoRepository = pedidoRepository;
            _locacaoRepository = locacaoRepository;
            _notificacaoPedidoRepository = notificacaoPedidoRepository;
            _entregadorRepository = entregadorRepository;
            _sqsRepository = sqsRepository;
        }

        public async Task CriarPedido(PedidoModel pedido)
        {
            try
            {
                pedido.Situation = OrderSituationEnum.Available;
                await _pedidoRepository.SQLInsert(pedido);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task NotificarCriacaoPedido(PedidoModel pedido)
        {
            if (pedido.Situation != OrderSituationEnum.Available)
                throw new DesaException(409, "Order is not available for notification");
            try
            {
                var notificacoesPedidoEnviadas = (await _notificacaoPedidoRepository.FindMongoNotificacaoByIdPedido(pedido.Id)).ToList();

                if (notificacoesPedidoEnviadas.Any(ntf => ntf.Aceita)
                    || notificacoesPedidoEnviadas.Any(ntf => ntf.Valida))
                    return;

                var locacoesAtivas = (await _locacaoRepository.GetLocacoesAtivas()).ToList();

                var pedidosAceitos = (await _pedidoRepository.GetTodosPedidosSituation(OrderSituationEnum.Accepted)).ToList();

                var entregadoresSemPedidos = locacoesAtivas.Where(locacao =>
                !pedidosAceitos.Any(pedidoA => pedidoA.IdEntregador.Equals(locacao.IdEntregador)));

                if (!entregadoresSemPedidos.Any())
                    return;

                var locadorEscolhido = entregadoresSemPedidos.OrderBy(x => new Random().Next()).FirstOrDefault();

                await EnviaNotificacaoPedidoSQS(
                    new NotificacaoPedidoModel()
                    {
                        DataNotificacao = DateTime.Now,
                        IdEntregador = locadorEscolhido.IdEntregador,
                        IdPedido = pedido.Id,
                        Valida = true,
                        Aceita = false
                    });
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public async Task<List<EntregadorModel>> EntregadoresNotificados(int idPedido)
        {
            return (await Task.WhenAll((await _notificacaoPedidoRepository.FindMongoNotificacaoByIdPedido(idPedido))
                .Select(notificacao => _entregadorRepository.SQLGetOneById(notificacao.IdEntregador))))
                .ToList();
        }

        public async Task GerenciarStatusPedido(UserModel user, int idPedido, OrderSituationEnum targetSituation)
        {
            var notificacaoPedido = await _notificacaoPedidoRepository.FindMongoNotificacaoByIdPedidoEIdEntregador(idPedido, (int)user.IdEntregador)
                ?? throw new DesaException(406, "This user was not notified of this order.");

            var pedido = await _pedidoRepository.SQLGetOneById(idPedido);

            if ((targetSituation == OrderSituationEnum.Accepted && (!notificacaoPedido.Valida || pedido.Situation != OrderSituationEnum.Available)) ||
                (targetSituation == OrderSituationEnum.Delivered && (!notificacaoPedido.Aceita || pedido.Situation != OrderSituationEnum.Accepted)))
                throw new DesaException(406, "This notification is no longer available.");

            notificacaoPedido.Aceita = true;
            pedido.IdEntregador = (int)user.IdEntregador;

            pedido.Situation = targetSituation;

            await _pedidoRepository.SQLUpdate(pedido);

            await _notificacaoPedidoRepository.UpsertMongoAsync(notificacaoPedido);
        }

        public async Task ValidaNotificacoesEnviadas()
        {
            try
            {
                foreach (var notif in await _notificacaoPedidoRepository.FindMongoNotificacaoValidasNaoAceitas())
                {
                    if (DateTime.Compare(DateTime.Now, notif.DataNotificacao.AddMinutes(3)) >= 0)
                    {
                        notif.Valida = false;
                        await _notificacaoPedidoRepository.UpsertMongoAsync(notif);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async Task EnviaNotificacaoPedidoSQS(NotificacaoPedidoModel notificacao)
        {
            var configurationManager = (new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)).Build();

            var queueName = configurationManager.GetSection("AWS.SQS")["QueueName"];

            if (string.IsNullOrEmpty(queueName))
                return;

            await _sqsRepository.SendMessageAsync(queueName, notificacao);
            await _notificacaoPedidoRepository.InsertMongoAsync(notificacao);
        }
    }
}
