using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Desa.Core.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using System.Collections;
using Desa.Core.Authentication.Resorces;
using System.Text.Json;

namespace Desa.Core.Repositories
{
    public abstract class BaseRepository<T> : IRepositoryModel<T>, IDisposable where T : class
    {
        public readonly IMongoDatabase MongoDatabase;
        public readonly DesaContext SqlDatabase;
        public bool _SaveChanges = true;
        public abstract string GetCollectionName<T>();

        public IMongoCollection<T> GetCollection<T>()
        {
            return MongoDatabase.GetCollection<T>(GetCollectionName<T>());
        }

        protected BaseRepository()
        {
            SqlDatabase = new DesaContext();
            MongoDatabase = GetMongoDatabase();
        }

        protected static IMongoDatabase GetMongoDatabase()
        {
            var configurationManager = (new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)).Build();

            var connectionString = configurationManager.GetConnectionString("Mongo");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Aplicação sem ConnectionString 'Mongo' ");
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase("Desa");
        }
        protected async Task<IMongoCollection<T>> GetOrCreateCollectionAsync(string? collectionName = null)
        {
            collectionName ??= GetCollectionName<T>();
            var mongoCollection = MongoDatabase.GetCollection<T>(collectionName);
            if (mongoCollection == null)
            {
                await MongoDatabase.CreateCollectionAsync(collectionName);
                mongoCollection = MongoDatabase.GetCollection<T>(collectionName);
            }

            return mongoCollection;
        }
        protected IMongoCollection<LogModel<T>> GetOrCreateLogCollection()
        {
            var collectionName = string.Format("{0}-log", GetCollectionName<T>());
            var mongoCollection = MongoDatabase.GetCollection<LogModel<T>>(collectionName);
            if (mongoCollection == null)
            {
                MongoDatabase.CreateCollection(collectionName);
                mongoCollection = MongoDatabase.GetCollection<LogModel<T>>(collectionName);
            }

            return mongoCollection;
        }

        public async Task<IEnumerable<T>> SQLGetAll()
        {
            return await SqlDatabase.Set<T>().ToListAsync();
        }
        public async Task<IEnumerable<T>> SQLGetAll(int offset, int limit)
        {
            return await SqlDatabase.Set<T>().Skip(offset).Take(limit).ToListAsync();
        }
        public async Task<T> SQLGetOneById(params object[] variable)
        {
            return await SqlDatabase.Set<T>().FindAsync(variable);
        }
        public async Task<T> SQLInsert(T obj)
        {
            SqlDatabase.Set<T>().Add(obj);
            if (_SaveChanges)
                await SQLSaveChanges(obj, "SQLInsert");
            return obj;
        }
        public async Task<T> SQLUpdate(T obj)
        {
            SqlDatabase.Entry(obj).State = EntityState.Modified;
            if (_SaveChanges)
                await SQLSaveChanges(obj, "SQLUpdate");
            return obj;
        }
        public async Task SQLDelete(T obj)
        {
            SqlDatabase.Set<T>().Remove(obj);
            if (_SaveChanges)
                await SQLSaveChanges(obj, "SQLDelete");
        }
        public async Task SQLDelete(params object[] variable)
        {
            var obj = SQLGetOneById(variable);
            await SQLDelete(obj);
        }
        private async Task SQLSaveChanges(T obj, string action)
        {
            await SqlDatabase.SaveChangesAsync();
            (GetOrCreateLogCollection()).InsertOne(new LogModel<T>() { ModelCollection = GetCollectionName<T>(), Action = action, ActionDate = DateTime.Now, Value = obj });
        }
        public void Dispose()
        {
            SqlDatabase.Dispose();
        }
    }
}
