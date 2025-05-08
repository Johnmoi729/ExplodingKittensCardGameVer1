using System;
using System.Collections.Generic;

namespace ExplodingKittens.GameEngine.Models
{
    /// <summary>
    /// Holds information about the current game status
    /// </summary>
    public class GameStatusInfo
    {
        public bool IsPlayerTurn { get; set; }
        public bool IsPlayerExploded { get; set; }
        public int TurnsUntilPlayerTurn { get; set; }
        public bool HasDefuseCard { get; set; }
        public int RemainingPlayers { get; set; }
        public int RemainingCards { get; set; }
        public bool CanPlayCombo { get; set; }
    }
}