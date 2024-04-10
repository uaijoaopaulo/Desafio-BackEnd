using Desa.Core.Repositories.Models;
using Desa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface IPedidoRepository : IRepositoryModel<PedidoModel>
    {
        Task<IEnumerable<PedidoModel>> GetTodosPedidosSituation(OrderSituationEnum situation);
    }
}
