using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Infrastructure.Data.Context;
using MongoDB.Driver;

namespace ExplodingKittens.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for User entity
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MongoDbContext context) : base(context.Users)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatsAsync(string id, int gamesPlayed, int gamesWon)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(id);
            var filter = Builders<User>.Filter.Eq("_id", objectId);
            var update = Builders<User>.Update
                .Set(u => u.GamesPlayed, gamesPlayed)
                .Set(u => u.GamesWon, gamesWon);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}