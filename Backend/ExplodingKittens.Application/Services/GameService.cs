using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Constants;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Services
{
    /// <summary>
    /// Implementation of game service
    /// </summary>
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository; // Add this field

        public GameService(
            IGameRepository gameRepository,
            IGameStateRepository gameStateRepository,
            IUserRepository userRepository,
            ICardRepository cardRepository) // Add this parameter)
        {
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository; // Initialize the field
        }

        public async Task<GameDto> CreateGameAsync(CreateGameDto createGameDto)
        {
            // Verify user exists
            var user = await _userRepository.GetByIdAsync(createGameDto.HostPlayerId);
            if (user == null)
            {
                throw new Exception("Host player not found");
            }

            // Create game
            var game = new Game
            {
                Name = createGameDto.Name,
                Players = new List<string> { createGameDto.HostPlayerId },
                Status = GameConstants.Waiting,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _gameRepository.AddAsync(game);

            // Return game DTO
            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Players = game.Players,
                Status = game.Status,
                CurrentPlayerId = game.CurrentPlayerId,
                TurnNumber = game.TurnNumber,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt
            };
        }

        public async Task<GameDto> GetGameByIdAsync(string gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Players = game.Players,
                Status = game.Status,
                CurrentPlayerId = game.CurrentPlayerId,
                TurnNumber = game.TurnNumber,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt
            };
        }

        public async Task<IEnumerable<GameDto>> GetActiveGamesAsync()
        {
            var games = await _gameRepository.GetActiveGamesAsync();
            return games.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Players = g.Players,
                Status = g.Status,
                CurrentPlayerId = g.CurrentPlayerId,
                TurnNumber = g.TurnNumber,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }

        public async Task<IEnumerable<GameDto>> GetGamesByPlayerIdAsync(string playerId)
        {
            var games = await _gameRepository.GetGamesByPlayerIdAsync(playerId);
            return games.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Players = g.Players,
                Status = g.Status,
                CurrentPlayerId = g.CurrentPlayerId,
                TurnNumber = g.TurnNumber,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }

        public async Task<bool> JoinGameAsync(string gameId, string playerId)
        {
            // Verify game exists and is in waiting state
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.Waiting)
            {
                throw new Exception("Game is not in waiting state");
            }

            if (game.Players.Count >= GameConstants.MaxPlayers)
            {
                throw new Exception("Game is full");
            }

            if (game.Players.Contains(playerId))
            {
                throw new Exception("Player already in game");
            }

            // Verify user exists
            var user = await _userRepository.GetByIdAsync(playerId);
            if (user == null)
            {
                throw new Exception("Player not found");
            }

            // Add player to game
            return await _gameRepository.AddPlayerToGameAsync(gameId, playerId);
        }

        public async Task<bool> StartGameAsync(string gameId)
        {
            // Verify game exists and is in waiting state
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            if (game.Status != GameConstants.Waiting)
            {
                throw new Exception("Game is not in waiting state");
            }

            if (game.Players.Count < GameConstants.MinPlayers)
            {
                throw new Exception("Not enough players to start game");
            }

            // Set game status to in progress
            await _gameRepository.UpdateGameStatusAsync(gameId, GameConstants.InProgress);

            // Set current player to a random player
            var random = new Random();
            var randomPlayerIndex = random.Next(game.Players.Count);
            var randomPlayerId = game.Players[randomPlayerIndex];
            await _gameRepository.UpdateCurrentPlayerAsync(gameId, randomPlayerId);

            // Update game's turn number to 1
            game.TurnNumber = 1;
            game.UpdatedAt = DateTime.UtcNow;
            await _gameRepository.UpdateAsync(gameId, game);

            // Create the game engine
            var gameEngine = new GameEngine.GameEngine();

            // Get all cards
            var allCards = await _cardRepository.GetAllAsync();

            // Initialize game state
            var gameState = await gameEngine.InitializeGameAsync(game, allCards);

            // Save the game state
            await _gameStateRepository.AddAsync(gameState);

            return true;
        }
    }
}