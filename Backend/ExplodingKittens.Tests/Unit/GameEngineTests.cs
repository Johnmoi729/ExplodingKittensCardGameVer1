using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.GameEngine;
using GameEngineClass = ExplodingKittens.GameEngine.GameEngine;
using Xunit;

namespace ExplodingKittens.Tests.Unit
{
    public class GameEngineTests
    {
        private readonly GameEngineClass _gameEngine;

        public GameEngineTests()
        {
            _gameEngine = new GameEngineClass();
        }

        [Fact]
        public async Task InitializeGameAsync_ShouldCreateValidGameState()
        {
            // Arrange
            var game = new Game
            {
                Id = "game1",
                Players = new List<string> { "player1", "player2" },
                Status = "waiting",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

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

        private IEnumerable<Card> CreateTestCards()
        {
            var cards = new List<Card>();

            // Create exploding kittens
            for (int i = 0; i < 4; i++)
            {
                cards.Add(new Card
                {
                    Id = $"exploding{i}",
                    Type = "exploding_kitten",
                    Name = "Exploding Kitten",
                    Effect = "You explode! Unless you have a defuse card.",
                    ImageUrl = "/images/exploding.png"
                });
            }

            // Create defuse cards
            for (int i = 0; i < 6; i++)
            {
                cards.Add(new Card
                {
                    Id = $"defuse{i}",
                    Type = "defuse",
                    Name = "Defuse",
                    Effect = "Save yourself from an exploding kitten.",
                    ImageUrl = "/images/defuse.png"
                });
            }

            // Create other cards
            string[] cardTypes = { "attack", "skip", "shuffle", "see_future", "favor" };

            foreach (var type in cardTypes)
            {
                for (int i = 0; i < 4; i++)
                {
                    cards.Add(new Card
                    {
                        Id = $"{type}{i}",
                        Type = type,
                        Name = GetCardName(type),
                        Effect = GetCardEffect(type),
                        ImageUrl = $"/images/{type}.png"
                    });
                }
            }

            // Create cat cards
            string[] catTypes = { "taco", "rainbow", "beard", "potato" };

            foreach (var cat in catTypes)
            {
                for (int i = 0; i < 4; i++)
                {
                    cards.Add(new Card
                    {
                        Id = $"cat_{cat}{i}",
                        Type = "cat_card",
                        Name = $"{cat.ToUpperFirst()} Cat",
                        Effect = "Use as a pair to steal a random card.",
                        ImageUrl = $"/images/cat_{cat}.png"
                    });
                }
            }

            return cards;
        }

        private string GetCardName(string type)
        {
            return type switch
            {
                "attack" => "Attack",
                "skip" => "Skip",
                "shuffle" => "Shuffle",
                "see_future" => "See the Future",
                "favor" => "Favor",
                _ => type
            };
        }

        private string GetCardEffect(string type)
        {
            return type switch
            {
                "attack" => "End your turn without drawing. Next player takes 2 turns.",
                "skip" => "End your turn without drawing a card.",
                "shuffle" => "Shuffle the draw pile.",
                "see_future" => "View the top 3 cards of the draw pile.",
                "favor" => "Force another player to give you a card.",
                _ => ""
            };
        }
    }

    // Extension method
    public static class StringExtensions
    {
        public static string ToUpperFirst(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}