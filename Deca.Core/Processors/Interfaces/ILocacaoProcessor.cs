using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors.Interfaces
{
    public interface ILocacaoProcessor
    {
        Task<IEnumerable<MotoModel>> ConsultarMotosDisponiveisParaLocacao();
        Task LocarMoto(UserModel user, int diasLocacao);
        Task<decimal> CalcularValorLocacao(UserModel user, DateTime dataDevolucao);
    }
}
