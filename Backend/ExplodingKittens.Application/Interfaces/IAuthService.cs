using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Interface for authentication service
    /// </summary>
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetUserByIdAsync(string userId);
    }
}