using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ExplodingKittens.Infrastructure.Data.Context
{
    /// <summary>
    /// MongoDB context for accessing the database
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Collections
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Game> Games => _database.GetCollection<Game>("Games");
        public IMongoCollection<GameState> GameStates => _database.GetCollection<GameState>("GameStates");
        public IMongoCollection<Card> Cards => _database.GetCollection<Card>("Cards");
        public IMongoCollection<PlayerAction> PlayerActions => _database.GetCollection<PlayerAction>("PlayerActions");
    }
}