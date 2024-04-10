using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories
{
    public class NotificacaoPedidoRepository : BaseRepository<NotificacaoPedidoModel>, INotificacaoPedidoRepository
    {
        const string CollectionName = "notificacao-pedidos";
        public override string GetCollectionName<T>()
        {
            return CollectionName;
        }

        public async Task<IEnumerable<NotificacaoPedidoModel>> FindMongoNotificacaoByIdPedido(int idPedido)
        {
            var filter = Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.IdPedido, idPedido);
            return (await GetCollection<NotificacaoPedidoModel>().FindAsync(filter)).ToEnumerable();
        }

        public async Task<IEnumerable<NotificacaoPedidoModel>> FindMongoNotificacaoValidasNaoAceitas()
        {
            var filter = Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.Valida, true)
                & Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.Aceita, false);
            return (await GetCollection<NotificacaoPedidoModel>().FindAsync(filter)).ToEnumerable();
        }

        public async Task<NotificacaoPedidoModel> FindMongoNotificacaoByIdPedidoEIdEntregador(int idPedido, int idEntregador)
        {
            var filter = Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.IdPedido, idPedido)
                & Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.IdEntregador, idEntregador);
            return (await GetCollection<NotificacaoPedidoModel>().FindAsync(filter)).SingleOrDefault();
        }

        public async Task InsertMongoAsync(NotificacaoPedidoModel model)
        {
            await GetCollection<NotificacaoPedidoModel>().InsertOneAsync(model);
        }
        public async Task UpsertMongoAsync(NotificacaoPedidoModel model)
        {
            var filter = Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.IdPedido, model.IdPedido)
                 & Builders<NotificacaoPedidoModel>.Filter.Eq(x => x.IdEntregador, model.IdEntregador); 
            await GetCollection<NotificacaoPedidoModel>().ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
        }
    }
}
