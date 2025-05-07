// File: Backend/ExplodingKittens.Infrastructure/Data/Initialization/DatabaseInitializer.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Constants;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Infrastructure.Data.Context;
using MongoDB.Driver;

namespace ExplodingKittens.Infrastructure.Data.Initialization
{
    /// <summary>
    /// Initializes the database with required data
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly MongoDbContext _context;

        public DatabaseInitializer(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initializes the database with required data
        /// </summary>
        public async Task InitializeAsync()
        {
            await InitializeCardsAsync();
        }

        /// <summary>
        /// Initializes the card collection with the full deck
        /// </summary>
        private async Task InitializeCardsAsync()
        {
            // Check if we already have cards
            var cardCount = await _context.Cards.CountDocumentsAsync(FilterDefinition<Card>.Empty);
            if (cardCount > 0)
            {
                return;
            }

            var cards = new List<Card>();

            // Add exploding kittens
            for (int i = 0; i < GameConstants.ExplodingKittenCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "exploding_kitten",
                    Name = "Exploding Kitten",
                    Effect = "You explode! Unless you have a defuse card.",
                    ImageUrl = "/images/exploding_kitten.png"
                });
            }

            // Add defuse cards
            for (int i = 0; i < GameConstants.DefuseCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "defuse",
                    Name = "Defuse",
                    Effect = "Save yourself from an exploding kitten.",
                    ImageUrl = "/images/defuse.png"
                });
            }

            // Add attack cards
            for (int i = 0; i < GameConstants.AttackCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "attack",
                    Name = "Attack",
                    Effect = "End your turn without drawing. Next player takes 2 turns.",
                    ImageUrl = "/images/attack.png"
                });
            }

            // Add skip cards
            for (int i = 0; i < GameConstants.SkipCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "skip",
                    Name = "Skip",
                    Effect = "End your turn without drawing a card.",
                    ImageUrl = "/images/skip.png"
                });
            }

            // Add shuffle cards
            for (int i = 0; i < GameConstants.ShuffleCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "shuffle",
                    Name = "Shuffle",
                    Effect = "Shuffle the draw pile.",
                    ImageUrl = "/images/shuffle.png"
                });
            }

            // Add see future cards
            for (int i = 0; i < GameConstants.SeeFutureCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "see_future",
                    Name = "See the Future",
                    Effect = "View the top 3 cards of the draw pile.",
                    ImageUrl = "/images/see_future.png"
                });
            }

            // Add favor cards
            for (int i = 0; i < GameConstants.FavorCount; i++)
            {
                cards.Add(new Card
                {
                    Type = "favor",
                    Name = "Favor",
                    Effect = "Force another player to give you a card of their choice.",
                    ImageUrl = "/images/favor.png"
                });
            }

            // Add cat cards
            string[] catTypes = { "taco", "rainbow", "beard", "melon", "potato" };

            foreach (var catType in catTypes)
            {
                for (int i = 0; i < GameConstants.CatCardCount; i++)
                {
                    cards.Add(new Card
                    {
                        Type = "cat_card",
                        Name = $"{catType.ToUpper()} Cat",
                        Effect = "Use as a pair to steal a random card.",
                        ImageUrl = $"/images/cat_{catType}.png"
                    });
                }
            }

            // Insert all cards
            await _context.Cards.InsertManyAsync(cards);
        }
    }
}