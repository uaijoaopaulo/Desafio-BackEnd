using Desa.Core.Exceptions;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Desa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors
{
    public class LocacaoProcessor : ILocacaoProcessor
    {
        protected readonly ILocacaoRepository _locacaoRepository;
        protected readonly IMotoRepository _motoRepository;
        protected readonly IEntregadorRepository _entregadorRepository;
        public LocacaoProcessor(ILocacaoRepository locacaoRepository, IMotoRepository motoRepository, IEntregadorRepository entregadorRepository)
        {
            _locacaoRepository = locacaoRepository;
            _motoRepository = motoRepository;            _entregadorRepository = entregadorRepository;

        }

        public async Task<IEnumerable<MotoModel>> ConsultarMotosDisponiveisParaLocacao()
        {
            try
            {
                var locacoes = await _locacaoRepository.GetLocacoesAtivas();
                var todasMotos = await _motoRepository.SQLGetAll();

                return todasMotos.Where(moto => !locacoes.Any(locacao => locacao.IdMoto == moto.Id && locacao.Ativo));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task LocarMoto(UserModel user, int diasLocacao)
        {
            try
            {
                var entregador = await _entregadorRepository.SQLGetOneById(user.IdEntregador);
                if (entregador.CNHType == CNHTypeEnum.B)
                    throw new DesaException(400, "CNHType not allowed to rent in this category");

                var motoDisponivel = ((await ConsultarMotosDisponiveisParaLocacao())?.FirstOrDefault()) 
                    ?? throw new DesaException(400, "No bikes available at the moment, try again later.");

                await _locacaoRepository.SQLInsert(new LocacaoModel()
                {
                    DataInicio = DateTime.UtcNow.AddDays(1), // O inicio da locação obrigatóriamente é o primeiro dia após a data de criação.
                    DataPrevisaoTermino = DateTime.UtcNow.AddDays(diasLocacao + 1),
                    IdEntregador = entregador.Id,
                    IdMoto = motoDisponivel.Id,
                    DiasLocacao = diasLocacao,
                    ValorLocacao = ValorLocacao(diasLocacao),
                    Ativo = true
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<decimal> CalcularValorLocacao(UserModel user, DateTime dataDevolucao)
        {
            try
            {
                var locacaoAtiva = await _locacaoRepository.GetLocacaoAtivaByEntregador((int)user.IdEntregador);
                if (locacaoAtiva == null)
                    throw new DesaException(404, "No active rental for this user.");

                var diferenca = (locacaoAtiva.DataPrevisaoTermino - dataDevolucao).Days;

                if(diferenca < locacaoAtiva.DiasLocacao && diferenca > 0)
                {
                    var valorMulta = new Dictionary<int, int>()
                    {
                        { 7, 20 },
                        { 15, 40 },
                        { 30, 60 }
                    };
                    var percentualMulta = (decimal)valorMulta[locacaoAtiva.DiasLocacao] / 100;
                    var diarias = locacaoAtiva.DiasLocacao - diferenca;
                    return (diarias * 50) + (ValorLocacao(locacaoAtiva.DiasLocacao) * percentualMulta);
                }
                return locacaoAtiva.ValorLocacao + (50 * Math.Abs(diferenca));
            }
            catch (Exception)
            {

                throw;
            }

        }

        public decimal ValorLocacao(int diasLocacao)
        {
            var dicValores = new Dictionary<int, decimal>()
            {
                { 7, 30.00m },
                { 15, 28.00m },
                { 30, 22.00m }
            };

            if (!dicValores.TryGetValue(diasLocacao, out decimal dicValor))
                throw new DesaException(400, "Days reported do not correspond to any available plan");

            return dicValor * diasLocacao;
        }
    }
}
