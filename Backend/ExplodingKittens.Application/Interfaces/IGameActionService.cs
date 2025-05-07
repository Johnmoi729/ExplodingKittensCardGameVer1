using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Interface for game action service
    /// </summary>
    public interface IGameActionService
    {
        Task<GameStateDto> PlayCardAsync(string gameId, PlayCardDto playCardDto);
        Task<GameStateDto> DrawCardAsync(string gameId, string playerId);
        Task<GameStateDto> PlayComboAsync(string gameId, PlayComboDto playComboDto);
        Task<GameStateDto> DefuseKittenAsync(string gameId, DefuseKittenDto defuseKittenDto);
        Task<SeeFutureResultDto> SeeFutureAsync(string gameId, string playerId);
    }
}