using System;
using System.Collections.Generic;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for Game entity
    /// </summary>
    public class GameDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Players { get; set; }
        public string Status { get; set; }
        public string CurrentPlayerId { get; set; }
        public int TurnNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}