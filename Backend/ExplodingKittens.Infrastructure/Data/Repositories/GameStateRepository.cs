using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Infrastructure.Data.Context;
using MongoDB.Driver;
using System.Collections.Generic;

namespace ExplodingKittens.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for GameState entity
    /// </summary>
    public class GameStateRepository : Repository<GameState>, IGameStateRepository
    {
        public GameStateRepository(MongoDbContext context) : base(context.GameStates)
        {
        }

        public async Task<GameState> GetByGameIdAsync(string gameId)
        {
            var filter = Builders<GameState>.Filter.Eq(gs => gs.GameId, gameId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDrawPileAsync(string gameStateId, List<string> drawPile)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameStateId);
            var filter = Builders<GameState>.Filter.Eq("_id", objectId);
            var update = Builders<GameState>.Update.Set(gs => gs.DrawPile, drawPile);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateDiscardPileAsync(string gameStateId, List<string> discardPile)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameStateId);
            var filter = Builders<GameState>.Filter.Eq("_id", objectId);
            var update = Builders<GameState>.Update.Set(gs => gs.DiscardPile, discardPile);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdatePlayerHandsAsync(string gameStateId, Dictionary<string, List<string>> playerHands)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameStateId);
            var filter = Builders<GameState>.Filter.Eq("_id", objectId);
            var update = Builders<GameState>.Update.Set(gs => gs.PlayerHands, playerHands);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddExplodedPlayerAsync(string gameStateId, string playerId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(gameStateId);
            var filter = Builders<GameState>.Filter.Eq("_id", objectId);
            var update = Builders<GameState>.Update.AddToSet(gs => gs.ExplodedPlayers, playerId);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}