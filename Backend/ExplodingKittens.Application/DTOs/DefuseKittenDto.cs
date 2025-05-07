using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for defusing an exploding kitten
    /// </summary>
    public class DefuseKittenDto
    {
        [Required]
        public string PlayerId { get; set; }

        [Required]
        public string DefuseCardId { get; set; }

        [Required]
        public int Position { get; set; } // Position to insert the exploding kitten in the draw pile
    }
}