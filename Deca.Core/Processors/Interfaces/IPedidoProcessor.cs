using Desa.Core.Repositories.Models;
using Desa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors.Interfaces
{
    public interface IPedidoProcessor
    {
        Task CriarPedido(PedidoModel pedido);
        Task NotificarCriacaoPedido(PedidoModel pedido);
        Task<List<EntregadorModel>> EntregadoresNotificados(int idPedido);
        Task GerenciarStatusPedido(UserModel user, int idPedido, OrderSituationEnum targetSituation);
        Task ValidaNotificacoesEnviadas();
    }
}
