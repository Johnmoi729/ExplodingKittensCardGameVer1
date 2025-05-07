using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new game
    /// </summary>
    public class CreateGameDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public string HostPlayerId { get; set; }
    }
}