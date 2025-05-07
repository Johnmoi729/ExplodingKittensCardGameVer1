using System;
using System.Collections.Generic;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for the game state
    /// </summary>
    public class GameStateDto
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public int DrawPileCount { get; set; }
        public List<CardDto> DiscardPile { get; set; }
        public Dictionary<string, List<CardDto>> PlayerHands { get; set; }
        public List<string> ExplodedPlayers { get; set; }
        public int AttackCount { get; set; }
        public string LastAction { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CurrentPlayerId { get; set; }
        public int TurnNumber { get; set; }
    }
}