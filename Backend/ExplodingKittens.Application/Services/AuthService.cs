using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using ExplodingKittens.Domain.Entities;

namespace ExplodingKittens.Application.Services
{
    /// <summary>
    /// Implementation of authentication service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService; // Use the interface

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username or email already exists
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUserByUsername != null)
            {
                throw new Exception("Username already exists");
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUserByEmail != null)
            {
                throw new Exception("Email already exists");
            }

            // Create user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                GamesPlayed = 0,
                GamesWon = 0
            };

            await _userRepository.AddAsync(user);

            // Return user DTO
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                GamesPlayed = user.GamesPlayed,
                GamesWon = user.GamesWon
            };
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Get user by username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }

            // Verify password
            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password");
            }

            // Generate and return token
            return _tokenService.GenerateToken(user);
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                GamesPlayed = user.GamesPlayed,
                GamesWon = user.GamesWon
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }
    }
}