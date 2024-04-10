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
    public class EntregadorRepository : BaseRepository<EntregadorModel>, IEntregadorRepository
    {

        const string CollectionName = "entregadores";
        public override string GetCollectionName<T>()
        {
            return CollectionName;
        }

        public async Task<EntregadorModel> GetByCNH(string CNH)
        {
            return await SqlDatabase.Entregadores.FirstOrDefaultAsync(_ => _.CNH.Equals(CNH));
        }
    }
}
