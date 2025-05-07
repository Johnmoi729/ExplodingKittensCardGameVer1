using System.Collections.Generic;
using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Infrastructure.Data.Context;
using MongoDB.Driver;

namespace ExplodingKittens.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Game entity
    /// </summary>
    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(MongoDbContext context) : base(context.Games)
        {
        }

        public async Task<IEnumerable<Game>> GetActiveGamesAsync()
        {
            var filter = Builders<Game>.Filter.Ne(g => g.Status, "completed");
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByPlayerIdAsync(string playerId)
        {
            var filter = Builders<Game>.Filter.AnyEq(g => g.Players, playerId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameId);
            var filter = Builders<Game>.Filter.Eq("_id", objectId);
            var update = Builders<Game>.Update.AddToSet(g => g.Players, playerId);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateGameStatusAsync(string gameId, string status)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameId);
            var filter = Builders<Game>.Filter.Eq("_id", objectId);
            var update = Builders<Game>.Update.Set(g => g.Status, status);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateCurrentPlayerAsync(string gameId, string playerId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameId);
            var filter = Builders<Game>.Filter.Eq("_id", objectId);
            var update = Builders<Game>.Update.Set(g => g.CurrentPlayerId, playerId);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}