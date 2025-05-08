using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Constants;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.GameEngine.Models;
using ExplodingKittens.Domain.Enums;

namespace ExplodingKittens.GameEngine
{
    /// <summary>
    /// Main game engine that handles game logic and state transitions
    /// </summary>
    public class GameEngine
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Initializes a new game with cards and initial state
        /// </summary>
        public async Task<GameState> InitializeGameAsync(Game game, IEnumerable<Card> allCards)
        {
            // Group cards by type
            var cardsByType = allCards.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.ToList());

            // Create draw pile
            var drawPile = new List<string>();
            var playerHands = new Dictionary<string, List<string>>();

            // Initialize player hands with empty lists
            foreach (var playerId in game.Players)
            {
                playerHands[playerId] = new List<string>();
            }

            // Deal defuse cards - one to each player
            var defuseCards = cardsByType["defuse"];
            foreach (var playerId in game.Players)
            {
                if (defuseCards.Count > 0)
                {
                    var defuseCard = defuseCards[0];
                    playerHands[playerId].Add(defuseCard.Id);
                    defuseCards.RemoveAt(0);
                }
            }

            // Add remaining defuse cards to the draw pile
            drawPile.AddRange(defuseCards.Select(c => c.Id));

            // Create initial deck without exploding kittens
            foreach (var cardType in cardsByType)
            {
                if (cardType.Key != "exploding_kitten" && cardType.Key != "defuse")
                {
                    drawPile.AddRange(cardType.Value.Select(c => c.Id));
                }
            }

            // Shuffle the draw pile
            ShuffleDeck(drawPile);

            // Deal 7 cards to each player
            for (int i = 0; i < GameConstants.InitialHandSize; i++)
            {
                foreach (var playerId in game.Players)
                {
                    if (drawPile.Count > 0)
                    {
                        var card = drawPile[0];
                        playerHands[playerId].Add(card);
                        drawPile.RemoveAt(0);
                    }
                }
            }

            // Add exploding kittens to the draw pile
            var explodingKittens = cardsByType["exploding_kitten"];
            var kittenCount = Math.Max(0, game.Players.Count - 1); // One fewer than the number of players

            for (int i = 0; i < kittenCount; i++)
            {
                if (i < explodingKittens.Count)
                {
                    drawPile.Add(explodingKittens[i].Id);
                }
            }

            // Shuffle the draw pile again with exploding kittens
            ShuffleDeck(drawPile);

            // Create initial game state
            var gameState = new GameState
            {
                GameId = game.Id,
                DrawPile = drawPile,
                DiscardPile = new List<string>(),
                PlayerHands = playerHands,
                ExplodedPlayers = new List<string>(),
                AttackCount = 0,
                LastAction = "Game initialized",
                UpdatedAt = DateTime.UtcNow
            };

            return gameState;
        }

        /// <summary>
        /// Process a player's turn when they play a card
        /// </summary>
        public async Task<GameState> PlayCardAsync(GameState gameState, Game game, string playerId, string cardId, string targetPlayerId = null, IEnumerable<Card> cards = null)
        {
            // Check if it's the player's turn
            if (game.CurrentPlayerId != playerId)
            {
                throw new InvalidOperationException("It's not your turn");
            }

            // Check if the player has the card
            if (!gameState.PlayerHands[playerId].Contains(cardId))
            {
                throw new InvalidOperationException("You don't have this card");
            }

            // Get the card details
            var card = cards.FirstOrDefault(c => c.Id == cardId);
            if (card == null)
            {
                throw new InvalidOperationException("Card not found");
            }

            // Remove the card from the player's hand
            gameState.PlayerHands[playerId].Remove(cardId);

            // Add the card to the discard pile
            gameState.DiscardPile.Add(cardId);

            // Apply card effect based on type
            switch (card.Type)
            {
                case "attack":
                    gameState = ApplyAttackEffect(gameState, game);
                    break;
                case "skip":
                    gameState = ApplySkipEffect(gameState, game);
                    break;
                case "shuffle":
                    gameState = ApplyShuffleEffect(gameState);
                    break;
                case "see_future":
                    gameState = ApplySeeFutureEffect(gameState);
                    break;
                case "favor":
                    gameState = ApplyFavorEffect(gameState, game, playerId, targetPlayerId);
                    break;
                default:
                    // For cat cards or other cards, just discard
                    gameState.LastAction = $"Player {playerId} played {card.Name}";
                    break;
            }

            gameState.UpdatedAt = DateTime.UtcNow;
            return gameState;
        }

        /// <summary>
        /// Process a player's turn when they draw a card
        /// </summary>
        public async Task<GameState> DrawCardAsync(GameState gameState, Game game, string playerId, Func<string, int, Task<bool>> insertExplodingKittenCallback = null)
        {
            // Check if it's the player's turn
            if (game.CurrentPlayerId != playerId)
            {
                throw new InvalidOperationException("It's not your turn");
            }

            // Check if there are cards in the draw pile
            if (gameState.DrawPile.Count == 0)
            {
                throw new InvalidOperationException("No cards left in the draw pile");
            }

            // Draw the top card
            var cardId = gameState.DrawPile[0];
            gameState.DrawPile.RemoveAt(0);

            // Check if it's an exploding kitten
            var card = (await GetCardsByIdsAsync(new List<string> { cardId })).FirstOrDefault();
            if (card.Type == "exploding_kitten")
            {
                // Check if the player has a defuse card
                var defuseCardId = gameState.PlayerHands[playerId].FirstOrDefault(c =>
                {
                    var playerCard = GetCardsByIdsAsync(new List<string> { c }).Result.FirstOrDefault();
                    return playerCard?.Type == "defuse";
                });

                if (defuseCardId != null)
                {
                    // Player has a defuse card
                    gameState.PlayerHands[playerId].Remove(defuseCardId);
                    gameState.DiscardPile.Add(defuseCardId);
                    gameState.LastAction = $"Player {playerId} defused an Exploding Kitten";

                    // Let the player insert the exploding kitten back into the draw pile
                    if (insertExplodingKittenCallback != null)
                    {
                        await insertExplodingKittenCallback(playerId, gameState.DrawPile.Count);
                    }
                    else
                    {
                        // Default behavior: insert randomly
                        var position = _random.Next(gameState.DrawPile.Count + 1);
                        gameState.DrawPile.Insert(position, cardId);
                    }
                }
                else
                {
                    // Player explodes
                    gameState.ExplodedPlayers.Add(playerId);
                    gameState.DiscardPile.Add(cardId);
                    gameState.LastAction = $"Player {playerId} exploded!";

                    // Check if the game is over
                    var remainingPlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();
                    if (remainingPlayers.Count <= 1)
                    {
                        // Game over
                        game.Status = GameConstants.Completed;
                        game.UpdatedAt = DateTime.UtcNow;

                        if (remainingPlayers.Count == 1)
                        {
                            gameState.LastAction = $"Game over! Player {remainingPlayers[0]} wins!";
                        }
                        else
                        {
                            gameState.LastAction = "Game over! It's a tie!";
                        }

                        return gameState;
                    }
                }
            }
            else
            {
                // Add the card to the player's hand
                gameState.PlayerHands[playerId].Add(cardId);
                gameState.LastAction = $"Player {playerId} drew a card";
            }

            // Move to the next player if not exploded and not an attack
            if (!gameState.ExplodedPlayers.Contains(playerId) && gameState.AttackCount == 0)
            {
                MoveToNextPlayer(game, gameState);
            }
            else if (gameState.AttackCount > 0)
            {
                // Decrement attack count
                gameState.AttackCount--;

                // If attack count is 0, move to the next player
                if (gameState.AttackCount == 0)
                {
                    MoveToNextPlayer(game, gameState);
                }
            }

            gameState.UpdatedAt = DateTime.UtcNow;
            return gameState;
        }

        /// <summary>
        /// Process a special combo (2 or 3 of a kind)
        /// </summary>
        public async Task<GameState> PlaySpecialComboAsync(GameState gameState, Game game, string playerId, List<string> cardIds, string targetPlayerId, IEnumerable<Card> cards = null)
        {
            // Check if it's the player's turn
            if (game.CurrentPlayerId != playerId)
            {
                throw new InvalidOperationException("It's not your turn");
            }

            // Check if the player has all the cards
            foreach (var cardId in cardIds)
            {
                if (!gameState.PlayerHands[playerId].Contains(cardId))
                {
                    throw new InvalidOperationException("You don't have all these cards");
                }
            }

            // Check if target player exists
            if (!gameState.PlayerHands.ContainsKey(targetPlayerId))
            {
                throw new InvalidOperationException("Target player not found");
            }

            // Get the cards
            var comboCards = (await GetCardsByIdsAsync(cardIds)).ToList();

            // Check if the cards form a valid combo
            if (comboCards.Select(c => c.Name).Distinct().Count() != 1)
            {
                throw new InvalidOperationException("Not a valid combo - cards must have the same name");
            }

            // Process the combo
            if (cardIds.Count == 2)
            {
                // Two of a kind - steal a random card
                if (gameState.PlayerHands[targetPlayerId].Count == 0)
                {
                    throw new InvalidOperationException("Target player has no cards");
                }

                // Remove the cards from the player's hand
                foreach (var cardId in cardIds)
                {
                    gameState.PlayerHands[playerId].Remove(cardId);
                    gameState.DiscardPile.Add(cardId);
                }

                // Get a random card from the target player
                var randomIndex = _random.Next(gameState.PlayerHands[targetPlayerId].Count);
                var stolenCardId = gameState.PlayerHands[targetPlayerId][randomIndex];

                // Transfer the card
                gameState.PlayerHands[targetPlayerId].RemoveAt(randomIndex);
                gameState.PlayerHands[playerId].Add(stolenCardId);

                gameState.LastAction = $"Player {playerId} played Two of a Kind and stole a card from Player {targetPlayerId}";
            }
            else if (cardIds.Count == 3)
            {
                // Three of a kind - name and take a specific card
                // In a real implementation, you would need a way for the player to specify which card they want
                // For now, let's assume they want the first card (or we could add a parameter)

                // Remove the cards from the player's hand
                foreach (var cardId in cardIds)
                {
                    gameState.PlayerHands[playerId].Remove(cardId);
                    gameState.DiscardPile.Add(cardId);
                }

                // Get the specified card from the target player
                if (gameState.PlayerHands[targetPlayerId].Count > 0)
                {
                    var stolenCardId = gameState.PlayerHands[targetPlayerId][0];

                    // Transfer the card
                    gameState.PlayerHands[targetPlayerId].RemoveAt(0);
                    gameState.PlayerHands[playerId].Add(stolenCardId);

                    gameState.LastAction = $"Player {playerId} played Three of a Kind and stole a specific card from Player {targetPlayerId}";
                }
                else
                {
                    gameState.LastAction = $"Player {playerId} played Three of a Kind but Player {targetPlayerId} had no cards";
                }
            }

            gameState.UpdatedAt = DateTime.UtcNow;
            return gameState;
        }

        /// <summary>
        /// Gets the current game status, including whose turn it is and available actions
        /// </summary>
        public GameStatusInfo GetGameStatus(GameState gameState, Game game, string requestingPlayerId)
        {
            var isPlayerTurn = game.CurrentPlayerId == requestingPlayerId;
            var isPlayerExploded = gameState.ExplodedPlayers.Contains(requestingPlayerId);
            var hasDefuseCard = gameState.PlayerHands.ContainsKey(requestingPlayerId) &&
                                 gameState.PlayerHands[requestingPlayerId].Any(c =>
                                    GetCardAsync(c).Result?.Type == "defuse");

            var activePlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();
            var playerPosition = activePlayers.IndexOf(requestingPlayerId);
            var currentPlayerPosition = activePlayers.IndexOf(game.CurrentPlayerId);
            var turnsUntilPlayerTurn = playerPosition <= currentPlayerPosition ?
                activePlayers.Count - (currentPlayerPosition - playerPosition) :
                playerPosition - currentPlayerPosition;

            if (turnsUntilPlayerTurn == activePlayers.Count)
            {
                turnsUntilPlayerTurn = 0;
            }

            return new GameStatusInfo
            {
                IsPlayerTurn = isPlayerTurn,
                IsPlayerExploded = isPlayerExploded,
                TurnsUntilPlayerTurn = isPlayerTurn ? 0 : turnsUntilPlayerTurn,
                HasDefuseCard = hasDefuseCard,
                RemainingPlayers = activePlayers.Count,
                RemainingCards = gameState.DrawPile.Count,
                CanPlayCombo = CanPlayCombo(gameState, requestingPlayerId)
            };
        }

        /// <summary>
        /// Checks if the player can play a combo (2 or 3 of a kind)
        /// </summary>
        private bool CanPlayCombo(GameState gameState, string playerId)
        {
            if (!gameState.PlayerHands.ContainsKey(playerId))
                return false;

            var hand = gameState.PlayerHands[playerId];
            var cardsByName = new Dictionary<string, int>();

            foreach (var cardId in hand)
            {
                var card = GetCardAsync(cardId).Result;
                if (card == null) continue;

                if (!cardsByName.ContainsKey(card.Name))
                    cardsByName[card.Name] = 0;

                cardsByName[card.Name]++;
            }

            return cardsByName.Any(kvp => kvp.Value >= 2);
        }

        /// <summary>
        /// Handles the end of the game
        /// </summary>
        public async Task<Game> HandleGameEndAsync(Game game, GameState gameState)
        {
            var activePlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();

            if (activePlayers.Count <= 1)
            {
                game.Status = "completed";

                if (activePlayers.Count == 1)
                {
                    var winnerId = activePlayers.First();
                    gameState.LastAction = $"Game over! Player {winnerId} wins!";

                    // Update winner stats
                    await UpdatePlayerStatsAsync(game.Players, winnerId);
                }
                else
                {
                    gameState.LastAction = "Game over! It's a tie!";
                }

                game.UpdatedAt = DateTime.UtcNow;
            }

            return game;
        }

        // Inside the GameEngine class, add this missing method:
        private async Task UpdatePlayerStatsAsync(List<string> players, string winnerId)
        {
            // This method would normally update player statistics in a database
            // In a real implementation, you would inject IUserRepository and update stats

            // For now, we'll leave this as a placeholder
            // In an actual implementation you would:
            // 1. Increment gamesPlayed for all participants
            // 2. Increment gamesWon for the winner

            await Task.CompletedTask; // Placeholder for async operation
        }

        #region Card Effect Methods

        private GameState ApplyAttackEffect(GameState gameState, Game game)
        {
            // End the current turn without drawing
            // Force the next player to take 2 turns
            gameState.AttackCount += 2;

            // Move to the next player
            MoveToNextPlayer(game, gameState);

            gameState.LastAction = $"Player {game.CurrentPlayerId} played Attack. The next player must take 2 turns.";

            return gameState;
        }

        private GameState ApplySkipEffect(GameState gameState, Game game)
        {
            // End the current turn without drawing
            MoveToNextPlayer(game, gameState);

            gameState.LastAction = $"Player {game.CurrentPlayerId} played Skip. Their turn is over.";

            return gameState;
        }

        private GameState ApplyShuffleEffect(GameState gameState)
        {
            // Shuffle the draw pile
            ShuffleDeck(gameState.DrawPile);

            gameState.LastAction = "Deck has been shuffled.";

            return gameState;
        }

        private GameState ApplySeeFutureEffect(GameState gameState)
        {
            // In a real implementation, you would need to show the top 3 cards to the player
            // Since this is just the backend, we'll just update the game state

            gameState.LastAction = "Player viewed the top 3 cards of the deck.";

            return gameState;
        }

        private GameState ApplyFavorEffect(GameState gameState, Game game, string playerId, string targetPlayerId)
        {
            // Check if target player exists
            if (string.IsNullOrEmpty(targetPlayerId) || !gameState.PlayerHands.ContainsKey(targetPlayerId))
            {
                throw new InvalidOperationException("Invalid target player");
            }

            // In a real implementation, you would need a way for the target player to choose a card
            // For now, let's assume they give the first card in their hand
            if (gameState.PlayerHands[targetPlayerId].Count > 0)
            {
                var cardId = gameState.PlayerHands[targetPlayerId][0];
                gameState.PlayerHands[targetPlayerId].RemoveAt(0);
                gameState.PlayerHands[playerId].Add(cardId);

                gameState.LastAction = $"Player {playerId} played Favor. Player {targetPlayerId} gave them a card.";
            }
            else
            {
                gameState.LastAction = $"Player {playerId} played Favor, but Player {targetPlayerId} had no cards to give.";
            }

            return gameState;
        }

        #endregion

        #region Helper Methods

        private void ShuffleDeck(List<string> deck)
        {
            // Fisher-Yates shuffle algorithm
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                string value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        private void MoveToNextPlayer(Game game, GameState gameState)
        {
            // Get the current player index
            var activePlayers = game.Players.Except(gameState.ExplodedPlayers).ToList();

            if (activePlayers.Count <= 1)
            {
                // Game is over, no need to change players
                return;
            }

            var currentIndex = activePlayers.FindIndex(p => p == game.CurrentPlayerId);

            // Move to the next active player
            var nextIndex = (currentIndex + 1) % activePlayers.Count;
            game.CurrentPlayerId = activePlayers[nextIndex];

            // Increment the turn number
            game.TurnNumber++;
            game.UpdatedAt = DateTime.UtcNow;
        }

        // Mock implementation - in your actual code, this would call your repository
        private Task<IEnumerable<Card>> GetCardsByIdsAsync(IEnumerable<string> cardIds)
        {
            // This is a placeholder. In your real implementation, you'd call your repository.
            // For now, return empty list to prevent compilation errors
            return Task.FromResult(Enumerable.Empty<Card>());
        }

        // Helper method for mocking in tests
        protected virtual Task<Card> GetCardAsync(string cardId)
        {
            // Implementation would call the card repository
            // For tests, this can be overridden
            return Task.FromResult<Card>(null);
        }

        #endregion
    }
}