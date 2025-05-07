using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExplodingKittens.Domain.Entities
{
    /// <summary>
    /// Represents an action taken by a player during the game
    /// </summary>
    public class PlayerAction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("gameId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GameId { get; set; }

        [BsonElement("playerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PlayerId { get; set; }

        [BsonElement("actionType")]
        public string ActionType { get; set; } // "play_card", "draw_card", etc.

        [BsonElement("cardsPlayed")]
        public List<string> CardsPlayed { get; set; } = new List<string>();

        [BsonElement("targetPlayerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TargetPlayerId { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}