using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;
using ExplodingKittens.Domain.Constants;
using ExplodingKittens.GameEngine;

namespace ExplodingKittens.Application.Services
{
    /// <summary>
    /// Implementation of game action service
    /// </summary>
    public class GameActionService : IGameActionService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly ICardRepository _cardRepository;
        private readonly GameEngine.GameEngine _gameEngine;

        public GameActionService(
            IGameRepository gameRepository,
            IGameStateRepository gameStateRepository,
            ICardRepository cardRepository)
        {
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _cardRepository = cardRepository;
            _gameEngine = new GameEngine.GameEngine();
        }

        public async Task<GameStateDto> PlayCardAsync(string gameId, PlayCardDto playCardDto)
        {
            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.InProgress)
            {
                throw new Exception("Game is not in progress");
            }

            // Get the game state
            var gameState = await _gameStateRepository.GetByGameIdAsync(gameId);
            if (gameState == null)
            {
                throw new Exception("Game state not found");
            }

            // Get the card
            var cards = await _cardRepository.GetCardsByIdsAsync(new[] { playCardDto.CardId });
            var card = cards.FirstOrDefault();
            if (card == null)
            {
                throw new Exception("Card not found");
            }

            // Process the card play
            gameState = await _gameEngine.PlayCardAsync(
                gameState,
                game,
                playCardDto.PlayerId,
                playCardDto.CardId,
                playCardDto.TargetPlayerId,
                cards);

            // Save the updated game state
            await _gameStateRepository.UpdateAsync(gameState.Id, gameState);

            // Save the updated game
            await _gameRepository.UpdateAsync(game.Id, game);

            // Return the updated game state
            return MapToGameStateDto(gameState, game);
        }

        public async Task<GameStateDto> DrawCardAsync(string gameId, string playerId)
        {
            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.InProgress)
            {
                throw new Exception("Game is not in progress");
            }

            // Get the game state
            var gameState = await _gameStateRepository.GetByGameIdAsync(gameId);
            if (gameState == null)
            {
                throw new Exception("Game state not found");
            }

            // Process the card draw
            gameState = await _gameEngine.DrawCardAsync(
                gameState,
                game,
                playerId,
                async (pid, maxPosition) =>
                {
                    // This is a placeholder for handling the exploding kitten insertion
                    // In a real implementation, you would need to wait for the player to make a choice
                    return true;
                });

            // Save the updated game state
            await _gameStateRepository.UpdateAsync(gameState.Id, gameState);

            // Save the updated game
            await _gameRepository.UpdateAsync(game.Id, game);

            // Return the updated game state
            return MapToGameStateDto(gameState, game);
        }

        public async Task<GameStateDto> PlayComboAsync(string gameId, PlayComboDto playComboDto)
        {
            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.InProgress)
            {
                throw new Exception("Game is not in progress");
            }

            // Get the game state
            var gameState = await _gameStateRepository.GetByGameIdAsync(gameId);
            if (gameState == null)
            {
                throw new Exception("Game state not found");
            }

            // Get the cards
            var cards = await _cardRepository.GetCardsByIdsAsync(playComboDto.CardIds);

            // Process the combo play
            gameState = await _gameEngine.PlaySpecialComboAsync(
                gameState,
                game,
                playComboDto.PlayerId,
                playComboDto.CardIds,
                playComboDto.TargetPlayerId,
                cards);

            // Save the updated game state
            await _gameStateRepository.UpdateAsync(gameState.Id, gameState);

            // Save the updated game
            await _gameRepository.UpdateAsync(game.Id, game);

            // Return the updated game state
            return MapToGameStateDto(gameState, game);
        }

        public async Task<GameStateDto> DefuseKittenAsync(string gameId, DefuseKittenDto defuseKittenDto)
        {
            // This is a placeholder for handling the defuse kitten action
            // In a real implementation, you would need to handle this more completely

            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            // Get the game state
            var gameState = await _gameStateRepository.GetByGameIdAsync(gameId);
            if (gameState == null)
            {
                throw new Exception("Game state not found");
            }

            // Return the updated game state
            return MapToGameStateDto(gameState, game);
        }

        public async Task<SeeFutureResultDto> SeeFutureAsync(string gameId, string playerId)
        {
            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.InProgress)
            {
                throw new Exception("Game is not in progress");
            }

            // Get the game state
            var gameState = await _gameStateRepository.GetByGameIdAsync(gameId);
            if (gameState == null)
            {
                throw new Exception("Game state not found");
            }

            // Check if it's the player's turn
            if (game.CurrentPlayerId != playerId)
            {
                throw new Exception("It's not your turn");
            }

            // Get the top 3 cards from the draw pile
            var topCardIds = gameState.DrawPile.Take(Math.Min(GameConstants.SeeFutureCardCount, gameState.DrawPile.Count)).ToList();
            var topCards = await _cardRepository.GetCardsByIdsAsync(topCardIds);

            // Return the top cards
            return new SeeFutureResultDto
            {
                TopCards = topCards.Select(c => new CardDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Name = c.Name,
                    Effect = c.Effect,
                    ImageUrl = c.ImageUrl
                }).ToList()
            };
        }

        private GameStateDto MapToGameStateDto(GameState gameState, Game game)
        {
            // This is a simplified mapping - in a real implementation, you would need to map more completely
            return new GameStateDto
            {
                Id = gameState.Id,
                GameId = gameState.GameId,
                DrawPileCount = gameState.DrawPile.Count,
                DiscardPile = new List<CardDto>(), // You would populate this with actual card data
                PlayerHands = new Dictionary<string, List<CardDto>>(), // You would populate this with actual card data
                ExplodedPlayers = gameState.ExplodedPlayers,
                AttackCount = gameState.AttackCount,
                LastAction = gameState.LastAction,
                UpdatedAt = gameState.UpdatedAt,
                CurrentPlayerId = game.CurrentPlayerId,
                TurnNumber = game.TurnNumber
            };
        }
    }
}