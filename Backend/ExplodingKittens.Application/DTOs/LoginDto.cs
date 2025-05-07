using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user login
    /// </summary>
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}