using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ExplodingKittens.Application.Services
{
    /// <summary>
    /// Implementation of game state service
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly ICardRepository _cardRepository;

        private readonly IHttpContextAccessor _httpContextAccessor; // Add this

        public GameStateService(
            IGameRepository gameRepository,
            IGameStateRepository gameStateRepository,
            ICardRepository cardRepository,
            IHttpContextAccessor httpContextAccessor) // Add this parameter
        {
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _cardRepository = cardRepository;
            _httpContextAccessor = httpContextAccessor; // Store the accessor
        }

        public async Task<GameStateDto> GetGameStateAsync(string gameId)
        {
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

            // Get all cards in the game state
            var allCardIds = new List<string>();

            // Add discard pile cards
            allCardIds.AddRange(gameState.DiscardPile);

            // Add cards from player hands
            foreach (var playerHand in gameState.PlayerHands)
            {
                allCardIds.AddRange(playerHand.Value);
            }

            // Get card details
            var cards = await _cardRepository.GetCardsByIdsAsync(allCardIds);
            var cardsDict = cards.ToDictionary(c => c.Id);

            // Build response DTO
            var response = new GameStateDto
            {
                Id = gameState.Id,
                GameId = gameState.GameId,
                DrawPileCount = gameState.DrawPile.Count,
                DiscardPile = gameState.DiscardPile
                    .Where(id => cardsDict.ContainsKey(id))
                    .Select(id => new CardDto
                    {
                        Id = id,
                        Type = cardsDict[id].Type,
                        Name = cardsDict[id].Name,
                        Effect = cardsDict[id].Effect,
                        ImageUrl = cardsDict[id].ImageUrl
                    })
                    .ToList(),
                PlayerHands = new Dictionary<string, List<CardDto>>(),
                ExplodedPlayers = gameState.ExplodedPlayers,
                AttackCount = gameState.AttackCount,
                LastAction = gameState.LastAction,
                UpdatedAt = gameState.UpdatedAt,
                CurrentPlayerId = game.CurrentPlayerId,
                TurnNumber = game.TurnNumber
            };

            // Build player hands
            // Then in your GetGameStateAsync method, replace:
            // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // With:
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            foreach (var playerHand in gameState.PlayerHands)
            {
                var playerId = playerHand.Key;
                var handCardIds = playerHand.Value;

                // Only include actual card details for the current player
                if (playerId == currentUserId)
                {
                    response.PlayerHands[playerId] = handCardIds
                        .Where(id => cardsDict.ContainsKey(id))
                        .Select(id => new CardDto
                        {
                            Id = id,
                            Type = cardsDict[id].Type,
                            Name = cardsDict[id].Name,
                            Effect = cardsDict[id].Effect,
                            ImageUrl = cardsDict[id].ImageUrl
                        })
                        .ToList();
                }
                else
                {
                    // For other players, just include a count
                    response.PlayerHands[playerId] = new List<CardDto>();
                }
            }

            return response;
        }
    }
}