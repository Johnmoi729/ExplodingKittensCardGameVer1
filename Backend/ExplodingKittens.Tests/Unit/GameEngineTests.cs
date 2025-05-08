using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.GameEngine;
using ExplodingKittens.GameEngine.Models;
using Xunit;
using Moq;

namespace ExplodingKittens.Tests.Unit
{
    public class GameEngineTests
    {
        private readonly TestableGameEngine _gameEngine;

        public GameEngineTests()
        {
            _gameEngine = new TestableGameEngine();
        }

        [Fact]
        public async Task InitializeGameAsync_ShouldCreateValidGameState()
        {
            // Arrange
            var game = CreateTestGame(2);
            var cards = CreateTestCards();

            // Act
            var gameState = await _gameEngine.InitializeGameAsync(game, cards);

            // Assert
            Assert.NotNull(gameState);
            Assert.Equal(game.Id, gameState.GameId);
            Assert.NotEmpty(gameState.DrawPile);
            Assert.Empty(gameState.DiscardPile);
            Assert.Equal(2, gameState.PlayerHands.Count);
            Assert.Contains("player1", gameState.PlayerHands.Keys);
            Assert.Contains("player2", gameState.PlayerHands.Keys);
            Assert.Equal(8, gameState.PlayerHands["player1"].Count); // 7 cards + 1 defuse
            Assert.Equal(8, gameState.PlayerHands["player2"].Count); // 7 cards + 1 defuse
            Assert.Empty(gameState.ExplodedPlayers);
            Assert.Equal(0, gameState.AttackCount);
            Assert.Equal("Game initialized", gameState.LastAction);
        }

        [Fact]
        public async Task PlayCardAsync_WithAttackCard_ShouldEndTurnAndForceNextPlayerToTakeTwoTurns()
        {
            // Arrange
            var game = CreateTestGame(2);
            var gameState = CreateTestGameState(game);
            var attackCard = CreateAttackCard();

            // Set up the mock to return the attack card
            _gameEngine.SetupCardMock(attackCard);

            // Make sure the attack card is in player1's hand
            gameState.PlayerHands["player1"].Add(attackCard.Id);

            // Act
            var updatedGameState = await _gameEngine.PlayCardAsync(
                gameState,
                game,
                "player1",
                attackCard.Id);

            // Assert
            Assert.Equal(2, updatedGameState.AttackCount);
            Assert.Equal("player2", game.CurrentPlayerId); // Turn should move to next player
            Assert.Contains(attackCard.Id, updatedGameState.DiscardPile);
            Assert.DoesNotContain(attackCard.Id, updatedGameState.PlayerHands["player1"]);
        }

        [Fact]
        public async Task PlayCardAsync_WithSkipCard_ShouldEndTurnWithoutDrawing()
        {
            // Arrange
            var game = CreateTestGame(2);
            var gameState = CreateTestGameState(game);
            var skipCard = CreateSkipCard();

            // Set up the mock to return the skip card
            _gameEngine.SetupCardMock(skipCard);

            // Make sure the skip card is in player1's hand
            gameState.PlayerHands["player1"].Add(skipCard.Id);

            // Act
            var updatedGameState = await _gameEngine.PlayCardAsync(
                gameState,
                game,
                "player1",
                skipCard.Id);

            // Assert
            Assert.Equal("player2", game.CurrentPlayerId); // Turn should move to next player
            Assert.Contains(skipCard.Id, updatedGameState.DiscardPile);
            Assert.DoesNotContain(skipCard.Id, updatedGameState.PlayerHands["player1"]);
        }

        [Fact]
        public async Task PlayCardAsync_WithFavorCard_ShouldRequestCardFromTargetPlayer()
        {
            // Arrange
            var game = CreateTestGame(3);
            var gameState = CreateTestGameState(game);
            var favorCard = CreateFavorCard();
            var targetPlayerId = "player2";
            var targetPlayerCard = "card123";

            // Set up the mock to return the favor card
            _gameEngine.SetupCardMock(favorCard);

            // Make sure the favor card is in player1's hand
            gameState.PlayerHands["player1"].Add(favorCard.Id);

            // Add a card to the target player's hand
            gameState.PlayerHands[targetPlayerId].Add(targetPlayerCard);

            // Act
            var updatedGameState = await _gameEngine.PlayCardAsync(
                gameState,
                game,
                "player1",
                favorCard.Id,
                targetPlayerId);

            // Assert
            Assert.Contains(favorCard.Id, updatedGameState.DiscardPile);
            Assert.DoesNotContain(favorCard.Id, updatedGameState.PlayerHands["player1"]);

            // The target player should have one fewer card
            Assert.Equal(0, updatedGameState.PlayerHands[targetPlayerId].Count);

            // The player who played the favor card should have the target player's card
            Assert.Contains(targetPlayerCard, updatedGameState.PlayerHands["player1"]);
        }

        [Fact]
        public async Task DrawCardAsync_WithNonExplodingCard_ShouldAddCardToPlayerHand()
        {
            // Arrange
            var game = CreateTestGame(2);
            var gameState = CreateTestGameState(game);
            var regularCard = "regular123";

            // Add a regular card to the draw pile
            gameState.DrawPile.Add(regularCard);

            // Create a mock card to return
            var mockCard = new Card { Id = regularCard, Type = "skip" };
            _gameEngine.SetupCardMock(mockCard);

            // Act
            var updatedGameState = await _gameEngine.DrawCardAsync(
                gameState,
                game,
                "player1");

            // Assert
            Assert.Contains(regularCard, updatedGameState.PlayerHands["player1"]);
            Assert.DoesNotContain(regularCard, updatedGameState.DrawPile);

            // Turn should move to the next player
            Assert.Equal("player2", game.CurrentPlayerId);
        }

        [Fact]
        public async Task DrawCardAsync_WithExplodingKitten_AndDefuse_ShouldAllowPlayerToDefuse()
        {
            // Arrange
            var game = CreateTestGame(2);
            var gameState = CreateTestGameState(game);
            var explodingCard = "exploding123";
            var defuseCard = "defuse123";

            // Add an exploding kitten to the draw pile
            gameState.DrawPile.Add(explodingCard);

            // Add a defuse card to the player's hand
            gameState.PlayerHands["player1"].Add(defuseCard);

            // Create mock cards to return
            var mockExplodingCard = new Card { Id = explodingCard, Type = "exploding_kitten" };
            var mockDefuseCard = new Card { Id = defuseCard, Type = "defuse" };

            _gameEngine.SetupCards(new Dictionary<string, Card> {
                { explodingCard, mockExplodingCard },
                { defuseCard, mockDefuseCard }
            });

            bool defuseCallbackCalled = false;

            // Act
            var updatedGameState = await _gameEngine.DrawCardAsync(
                gameState,
                game,
                "player1",
                async (playerId, maxPosition) =>
                {
                    defuseCallbackCalled = true;
                    Assert.Equal("player1", playerId);
                    Assert.Equal(0, maxPosition); // Empty draw pile

                    // Insert at position 0
                    gameState.DrawPile.Insert(0, explodingCard);
                    return true;
                });

            // Assert
            Assert.True(defuseCallbackCalled);
            Assert.DoesNotContain(defuseCard, updatedGameState.PlayerHands["player1"]);
            Assert.Contains(defuseCard, updatedGameState.DiscardPile);
            Assert.Contains(explodingCard, updatedGameState.DrawPile);
            Assert.DoesNotContain("player1", updatedGameState.ExplodedPlayers);
        }

        [Fact]
        public async Task DrawCardAsync_WithExplodingKitten_AndNoDefuse_ShouldExplodePlayer()
        {
            // Arrange
            var game = CreateTestGame(3);
            var gameState = CreateTestGameState(game);
            var explodingCard = "exploding123";

            // Add an exploding kitten to the draw pile
            gameState.DrawPile.Add(explodingCard);

            // Create a mock exploding card to return
            var mockExplodingCard = new Card { Id = explodingCard, Type = "exploding_kitten" };
            _gameEngine.SetupCardMock(mockExplodingCard);

            // Act
            var updatedGameState = await _gameEngine.DrawCardAsync(
                gameState,
                game,
                "player1");

            // Assert
            Assert.Contains("player1", updatedGameState.ExplodedPlayers);
            Assert.Contains(explodingCard, updatedGameState.DiscardPile);

            // Turn should move to the next player
            Assert.Equal("player2", game.CurrentPlayerId);

            // Game should not be over with 3 players - player1 exploded, player2 and player3 still alive
            Assert.Equal("in_progress", game.Status);
        }

        [Fact]
        public async Task PlayComboAsync_WithTwoOfAKind_ShouldStealRandomCard()
        {
            // Arrange
            var game = CreateTestGame(2);
            var gameState = CreateTestGameState(game);
            var cardId1 = "cat1";
            var cardId2 = "cat2";
            var targetCard = "target123";

            // Add cards to hands
            gameState.PlayerHands["player1"].Add(cardId1);
            gameState.PlayerHands["player1"].Add(cardId2);
            gameState.PlayerHands["player2"].Add(targetCard);

            // Create mock cards
            var catCard1 = new Card { Id = cardId1, Type = "cat_card", Name = "Taco Cat" };
            var catCard2 = new Card { Id = cardId2, Type = "cat_card", Name = "Taco Cat" };

            _gameEngine.SetupCards(new Dictionary<string, Card> {
                { cardId1, catCard1 },
                { cardId2, catCard2 }
            });

            // Act
            var updatedGameState = await _gameEngine.PlaySpecialComboAsync(
                gameState,
                game,
                "player1",
                new List<string> { cardId1, cardId2 },
                "player2",
                new List<Card> { catCard1, catCard2 });

            // Assert
            Assert.DoesNotContain(cardId1, updatedGameState.PlayerHands["player1"]);
            Assert.DoesNotContain(cardId2, updatedGameState.PlayerHands["player1"]);
            Assert.Contains(cardId1, updatedGameState.DiscardPile);
            Assert.Contains(cardId2, updatedGameState.DiscardPile);

            // Player1 should now have the target card
            Assert.Contains(targetCard, updatedGameState.PlayerHands["player1"]);

            // Player2 should no longer have the target card
            Assert.DoesNotContain(targetCard, updatedGameState.PlayerHands["player2"]);
        }

        [Fact]
        public void GetGameStatus_ShouldReturnCorrectStatus()
        {
            // Arrange
            var game = CreateTestGame(3);
            var gameState = CreateTestGameState(game);
            var defuseCard = "defuse123";

            // Add a defuse card to player1's hand
            gameState.PlayerHands["player1"].Add(defuseCard);

            // Create a mock defuse card
            var mockDefuseCard = new Card { Id = defuseCard, Type = "defuse" };
            _gameEngine.SetupCardMock(mockDefuseCard);

            // Act
            var status = _gameEngine.GetGameStatus(gameState, game, "player1");

            // Assert
            Assert.True(status.IsPlayerTurn); // player1 is the current player
            Assert.False(status.IsPlayerExploded);
            Assert.Equal(0, status.TurnsUntilPlayerTurn); // It's player1's turn
            Assert.True(status.HasDefuseCard);
            Assert.Equal(3, status.RemainingPlayers);
            Assert.Equal(0, status.RemainingCards);
        }

        private Game CreateTestGame(int numPlayers)
        {
            var players = new List<string>();
            for (int i = 1; i <= numPlayers; i++)
            {
                players.Add($"player{i}");
            }

            return new Game
            {
                Id = "testGameId",
                Name = "Test Game",
                Players = players,
                Status = "in_progress",
                CurrentPlayerId = "player1", // First player starts
                TurnNumber = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private GameState CreateTestGameState(Game game)
        {
            var playerHands = new Dictionary<string, List<string>>();
            foreach (var playerId in game.Players)
            {
                playerHands[playerId] = new List<string>();
            }

            return new GameState
            {
                Id = "testStateId",
                GameId = game.Id,
                DrawPile = new List<string>(),
                DiscardPile = new List<string>(),
                PlayerHands = playerHands,
                ExplodedPlayers = new List<string>(),
                AttackCount = 0,
                LastAction = "Game started",
                UpdatedAt = DateTime.UtcNow
            };
        }

        private Card CreateAttackCard()
        {
            return new Card
            {
                Id = "attack1",
                Type = "attack",
                Name = "Attack",
                Effect = "End your turn without drawing. Next player takes 2 turns."
            };
        }

        private Card CreateSkipCard()
        {
            return new Card
            {
                Id = "skip1",
                Type = "skip",
                Name = "Skip",
                Effect = "End your turn without drawing a card."
            };
        }

        private Card CreateFavorCard()
        {
            return new Card
            {
                Id = "favor1",
                Type = "favor",
                Name = "Favor",
                Effect = "Force another player to give you a card."
            };
        }

        private IEnumerable<Card> CreateTestCards()
        {
            var cards = new List<Card>();

            // Add defuse cards
            for (int i = 0; i < 6; i++)
            {
                cards.Add(new Card
                {
                    Id = $"defuse{i}",
                    Type = "defuse",
                    Name = "Defuse",
                    Effect = "Save yourself from an exploding kitten."
                });
            }

            // Add exploding kittens
            for (int i = 0; i < 4; i++)
            {
                cards.Add(new Card
                {
                    Id = $"exploding{i}",
                    Type = "exploding_kitten",
                    Name = "Exploding Kitten",
                    Effect = "You explode unless you have a defuse card."
                });
            }

            // Add other card types
            string[] cardTypes = { "attack", "skip", "shuffle", "see_future", "favor" };

            foreach (var type in cardTypes)
            {
                for (int i = 0; i < 4; i++)
                {
                    cards.Add(new Card
                    {
                        Id = $"{type}{i}",
                        Type = type,
                        Name = FirstLetterToUpper(type),
                        Effect = $"Effect of {type}"
                    });
                }
            }

            // Add cat cards
            string[] catTypes = { "taco", "rainbow", "beard", "melon", "potato" };

            foreach (var cat in catTypes)
            {
                for (int i = 0; i < 4; i++)
                {
                    cards.Add(new Card
                    {
                        Id = $"cat_{cat}{i}",
                        Type = "cat_card",
                        Name = $"{FirstLetterToUpper(cat)} Cat",
                        Effect = "Use as a pair to steal a random card."
                    });
                }
            }

            return cards;
        }

        private string FirstLetterToUpper(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length == 1)
                return str.ToUpper();

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }

    /// <summary>
    /// A testable version of GameEngine that allows mocking card retrieval
    /// </summary>
    public class TestableGameEngine : GameEngine.GameEngine
    {
        private Dictionary<string, Card> _mockCards = new Dictionary<string, Card>();

        public void SetupCardMock(Card card)
        {
            _mockCards[card.Id] = card;
        }

        public void SetupCards(Dictionary<string, Card> cards)
        {
            foreach (var card in cards)
            {
                _mockCards[card.Key] = card.Value;
            }
        }

        protected override Task<Card> GetCardAsync(string cardId)
        {
            if (_mockCards.TryGetValue(cardId, out var card))
            {
                return Task.FromResult(card);
            }

            return Task.FromResult<Card>(null);
        }
    }
}