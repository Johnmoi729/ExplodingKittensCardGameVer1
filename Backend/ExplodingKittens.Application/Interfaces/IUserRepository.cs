using System.Threading.Tasks;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Repository interface for User-specific operations
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> UpdateStatsAsync(string id, int gamesPlayed, int gamesWon);
    }
}