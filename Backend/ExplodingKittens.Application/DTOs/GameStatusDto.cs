using System;
using System.Collections.Generic;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for game status
    /// </summary>
    public class GameStatusDto
    {
        public bool IsPlayerTurn { get; set; }
        public bool IsPlayerExploded { get; set; }
        public int TurnsUntilPlayerTurn { get; set; }
        public bool HasDefuseCard { get; set; }
        public int RemainingPlayers { get; set; }
        public int RemainingCards { get; set; }
        public bool CanPlayCombo { get; set; }
        public string GameStatus { get; set; }
        public string LastAction { get; set; }
    }
}