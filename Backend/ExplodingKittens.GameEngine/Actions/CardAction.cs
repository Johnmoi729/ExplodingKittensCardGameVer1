using System;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.GameEngine.Actions
{
    /// <summary>
    /// Base class for card actions
    /// </summary>
    public abstract class CardAction
    {
        public abstract string Name { get; }

        /// <summary>
        /// Execute the card action
        /// </summary>
        public abstract GameState Execute(GameState gameState, Game game, string playerId, string targetPlayerId = null);

        /// <summary>
        /// Validate if the action can be performed
        /// </summary>
        public abstract bool CanExecute(GameState gameState, Game game, string playerId, string targetPlayerId = null);
    }
}