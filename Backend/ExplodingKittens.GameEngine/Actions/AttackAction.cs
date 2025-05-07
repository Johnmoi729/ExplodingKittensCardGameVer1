using System;
using System.Linq;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.GameEngine.Actions
{
    /// <summary>
    /// Implementation of the Attack card
    /// </summary>
    public class AttackAction : CardAction
    {
        public override string Name => "Attack";

        public override GameState Execute(GameState gameState, Game game, string playerId, string targetPlayerId = null)
        {
            if (!CanExecute(gameState, game, playerId))
            {
                throw new InvalidOperationException("Cannot execute Attack action");
            }

            // End the current turn without drawing
            // Force the next player to take 2 turns
            gameState.AttackCount += 2;

            // Find the next non-exploded player
            var activePlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();
            var currentIndex = activePlayers.IndexOf(playerId);
            var nextIndex = (currentIndex + 1) % activePlayers.Count;
            var nextPlayerId = activePlayers[nextIndex];

            // Set the next player
            game.CurrentPlayerId = nextPlayerId;

            // Update the game state
            gameState.LastAction = $"Player {playerId} played Attack. Player {nextPlayerId} must take 2 turns.";
            gameState.UpdatedAt = DateTime.UtcNow;

            return gameState;
        }

        public override bool CanExecute(GameState gameState, Game game, string playerId, string targetPlayerId = null)
        {
            // Check if it's the player's turn
            if (game.CurrentPlayerId != playerId)
            {
                return false;
            }

            // Check if the player has exploded
            if (gameState.ExplodedPlayers.Contains(playerId))
            {
                return false;
            }

            // Check if there are at least 2 active players
            var activePlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();
            if (activePlayers.Count < 2)
            {
                return false;
            }

            return true;
        }
    }
}