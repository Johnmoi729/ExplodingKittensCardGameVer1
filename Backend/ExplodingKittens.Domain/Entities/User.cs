using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExplodingKittens.Domain.Entities
{
    /// <summary>
    /// Represents a user in the Exploding Kittens game
    /// </summary>
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("gamesPlayed")]
        public int GamesPlayed { get; set; }

        [BsonElement("gamesWon")]
        public int GamesWon { get; set; }
    }
}