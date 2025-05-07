// Backend/ExplodingKittens.Infrastructure/Data/Repositories/CardRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Domain.Enums;
using ExplodingKittens.Infrastructure.Data.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExplodingKittens.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Card entity
    /// </summary>
    public class CardRepository : Repository<Card>, ICardRepository
    {
        public CardRepository(MongoDbContext context) : base(context.Cards)
        {
        }

        public async Task<IEnumerable<Card>> GetCardsByTypeAsync(CardType type)
        {
            var filter = Builders<Card>.Filter.Eq(c => c.Type, type.ToString());
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Card>> GetCardsByIdsAsync(IEnumerable<string> ids)
        {
            var objectIds = new List<ObjectId>();
            foreach (var id in ids)
            {
                objectIds.Add(ObjectId.Parse(id));
            }

            var filter = Builders<Card>.Filter.In("_id", objectIds);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<Card> GetRandomCardAsync()
        {
            // MongoDB doesn't have a direct way to get a random document
            // We'll use the aggregation framework with $sample operator
            var pipeline = new[]
            {
                new BsonDocument("$sample", new BsonDocument("size", 1))
            };

            var result = await _collection.AggregateAsync<Card>(pipeline);
            return await result.FirstOrDefaultAsync();
        }
    }
}