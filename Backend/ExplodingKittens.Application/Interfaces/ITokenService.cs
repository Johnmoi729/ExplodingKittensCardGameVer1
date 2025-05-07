// Backend/ExplodingKittens.Application/Interfaces/ITokenService.cs
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Interface for token generation service
    /// </summary>
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}