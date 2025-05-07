using System.Collections.Generic;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Interface for game service
    /// </summary>
    public interface IGameService
    {
        Task<GameDto> CreateGameAsync(CreateGameDto createGameDto);
        Task<GameDto> GetGameByIdAsync(string gameId);
        Task<IEnumerable<GameDto>> GetActiveGamesAsync();
        Task<IEnumerable<GameDto>> GetGamesByPlayerIdAsync(string playerId);
        Task<bool> JoinGameAsync(string gameId, string playerId);
        Task<bool> StartGameAsync(string gameId);
    }
}