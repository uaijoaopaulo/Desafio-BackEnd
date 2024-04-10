using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Interfaces;
using Desa.Models;
using Desa.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Worker.Services
{
    public class PedidoService : IPedidoService
    {
        protected readonly IPedidoRepository _pedidoRepository;
        protected readonly ILocacaoRepository _locacaoRepository;
        protected readonly INotificacaoPedidoRepository _notificacaoPedidoRepository;
        protected readonly IEntregadorRepository _entregadorRepository;
        protected readonly ISqsRepository _sqsRepository;
        protected readonly IPedidoProcessor _pedidoProcessor;

        public PedidoService(IPedidoRepository pedidoRepository, ILocacaoRepository locacaoRepository, INotificacaoPedidoRepository notificacaoPedidoRepository, IEntregadorRepository entregadorRepository, ISqsRepository sqsRepository, IPedidoProcessor pedidoProcessor)
        {
            _pedidoRepository = pedidoRepository;
            _locacaoRepository = locacaoRepository;
            _notificacaoPedidoRepository = notificacaoPedidoRepository;
            _entregadorRepository = entregadorRepository;
            _sqsRepository = sqsRepository;
            _pedidoProcessor = pedidoProcessor;
        }

        public async Task DoAsync()
        {
            try
            {
                await _pedidoProcessor.ValidaNotificacoesEnviadas();

                var pedidos = await _pedidoRepository.GetTodosPedidosSituation(OrderSituationEnum.Available);
                foreach (var pedido in pedidos)
                {
                    await _pedidoProcessor.NotificarCriacaoPedido(pedido);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //talvez utilizar raygun
            }
        }
    }
}
