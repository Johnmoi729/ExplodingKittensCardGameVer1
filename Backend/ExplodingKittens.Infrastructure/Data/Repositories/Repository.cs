using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ExplodingKittens.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Generic repository implementation for MongoDB
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public Repository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}