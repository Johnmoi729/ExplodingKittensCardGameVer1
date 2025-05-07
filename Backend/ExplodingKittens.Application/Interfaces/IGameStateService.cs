using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Interface for game state service
    /// </summary>
    public interface IGameStateService
    {
        Task<GameStateDto> GetGameStateAsync(string gameId);
    }
}