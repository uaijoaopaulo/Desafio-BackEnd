using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Desa.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories
{
    public class PedidoRepository : BaseRepository<PedidoModel>, IPedidoRepository
    {
        const string CollectionName = "pedidos";
        public override string GetCollectionName<T>()
        {
            return CollectionName;
        }

        public async Task<IEnumerable<PedidoModel>> GetTodosPedidosSituation(OrderSituationEnum situation)
        {
            return await SqlDatabase.Set<PedidoModel>().Where(pedido => pedido.Situation == situation).ToListAsync();
        }
    }
}
