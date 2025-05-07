using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExplodingKittens.Domain.Entities
{
    /// <summary>
    /// Represents the current state of a game
    /// </summary>
    public class GameState
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("gameId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GameId { get; set; }

        [BsonElement("drawPile")]
        public List<string> DrawPile { get; set; } = new List<string>();

        [BsonElement("discardPile")]
        public List<string> DiscardPile { get; set; } = new List<string>();

        [BsonElement("playerHands")]
        public Dictionary<string, List<string>> PlayerHands { get; set; } = new Dictionary<string, List<string>>();

        [BsonElement("explodedPlayers")]
        public List<string> ExplodedPlayers { get; set; } = new List<string>();

        [BsonElement("attackCount")]
        public int AttackCount { get; set; }

        [BsonElement("lastAction")]
        public string LastAction { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}