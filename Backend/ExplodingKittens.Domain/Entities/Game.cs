using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExplodingKittens.Domain.Entities
{
    /// <summary>
    /// Represents a game of Exploding Kittens
    /// </summary>
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("players")]
        public List<string> Players { get; set; } = new List<string>();

        [BsonElement("status")]
        public string Status { get; set; } // "waiting", "in_progress", "completed"

        [BsonElement("currentPlayerId")]
        public string CurrentPlayerId { get; set; }

        [BsonElement("turnNumber")]
        public int TurnNumber { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}