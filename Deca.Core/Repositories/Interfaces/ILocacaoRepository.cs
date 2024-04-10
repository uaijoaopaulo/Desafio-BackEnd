using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface ILocacaoRepository : IRepositoryModel<LocacaoModel>
    {
        Task<IEnumerable<LocacaoModel>> GetLocacoesAtivas();
        Task<LocacaoModel> GetLocacaoAtivaByMoto(int idMoto);
        Task<LocacaoModel> GetLocacaoAtivaByEntregador(int idEntregador);
    }
}
