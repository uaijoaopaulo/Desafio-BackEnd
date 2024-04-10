using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface IRepositoryModel<T> where T : class
    {
        IMongoCollection<T> GetCollection<T>();
        Task<IEnumerable<T>> SQLGetAll();
        Task<IEnumerable<T>> SQLGetAll(int offset, int limit);
        Task<T> SQLGetOneById(params object[] variable);
        Task<T> SQLInsert(T obj);
        Task<T> SQLUpdate(T obj);
        Task SQLDelete(T obj);
        Task SQLDelete(params object[] variable);
    }
}
