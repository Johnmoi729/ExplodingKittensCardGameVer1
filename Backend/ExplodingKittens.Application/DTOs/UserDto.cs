using System;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for User entity
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
    }
}