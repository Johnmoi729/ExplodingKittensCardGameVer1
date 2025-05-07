using System.Collections.Generic;
using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Repository interface for Game-specific operations
    /// </summary>
    public interface IGameRepository : IRepository<Game>
    {
        Task<IEnumerable<Game>> GetActiveGamesAsync();
        Task<IEnumerable<Game>> GetGamesByPlayerIdAsync(string playerId);
        Task<bool> AddPlayerToGameAsync(string gameId, string playerId);
        Task<bool> UpdateGameStatusAsync(string gameId, string status);
        Task<bool> UpdateCurrentPlayerAsync(string gameId, string playerId);
    }
}