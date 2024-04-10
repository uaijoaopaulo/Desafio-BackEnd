using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface INotificacaoPedidoRepository : IRepositoryModel<NotificacaoPedidoModel>
    {
        Task<IEnumerable<NotificacaoPedidoModel>> FindMongoNotificacaoByIdPedido(int idPedido);
        Task<IEnumerable<NotificacaoPedidoModel>> FindMongoNotificacaoValidasNaoAceitas();
        Task<NotificacaoPedidoModel> FindMongoNotificacaoByIdPedidoEIdEntregador(int idPedido, int idEntregador);
        Task InsertMongoAsync(NotificacaoPedidoModel model);
        Task UpsertMongoAsync(NotificacaoPedidoModel model);
    }
}
