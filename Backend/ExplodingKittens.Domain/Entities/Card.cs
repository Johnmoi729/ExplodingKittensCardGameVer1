using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExplodingKittens.Domain.Entities
{
    /// <summary>
    /// Represents a card in the Exploding Kittens game
    /// </summary>
    public class Card
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } // "exploding_kitten", "defuse", "attack", etc.

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("effect")]
        public string Effect { get; set; }

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }
    }
}