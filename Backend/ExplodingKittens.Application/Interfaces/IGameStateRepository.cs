using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Repository interface for GameState-specific operations
    /// </summary>
    public interface IGameStateRepository : IRepository<GameState>
    {
        Task<GameState> GetByGameIdAsync(string gameId);
        Task<bool> UpdateDrawPileAsync(string gameStateId, List<string> drawPile);
        Task<bool> UpdateDiscardPileAsync(string gameStateId, List<string> discardPile);
        Task<bool> UpdatePlayerHandsAsync(string gameStateId, Dictionary<string, List<string>> playerHands);
        Task<bool> AddExplodedPlayerAsync(string gameStateId, string playerId);
    }
}