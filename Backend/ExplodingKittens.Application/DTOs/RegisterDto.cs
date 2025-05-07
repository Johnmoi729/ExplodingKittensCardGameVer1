using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user registration
    /// </summary>
    public class RegisterDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}