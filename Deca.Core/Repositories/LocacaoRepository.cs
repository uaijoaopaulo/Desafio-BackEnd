using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories
{
    public class LocacaoRepository : BaseRepository<LocacaoModel>, ILocacaoRepository
    {
        const string CollectionName = "locacoes";
        public override string GetCollectionName<T>()
        {
            return CollectionName;
        }

        public async Task<IEnumerable<LocacaoModel>> GetLocacoesAtivas()
        {
            return await SqlDatabase.Locacoes.Where(locacao => locacao.Ativo == true).ToListAsync();
        }

        public async Task<LocacaoModel> GetLocacaoAtivaByMoto(int idMoto)
        {
            return await SqlDatabase.Locacoes.FirstOrDefaultAsync(locacao => locacao.IdMoto.Equals(idMoto) && locacao.Ativo);
        }

        public async Task<LocacaoModel> GetLocacaoAtivaByEntregador(int idEntregador)
        {
            return await SqlDatabase.Locacoes.FirstOrDefaultAsync(locacao => locacao.IdEntregador.Equals(idEntregador) && locacao.Ativo);
        }
    }
}
