using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for drawing a card
    /// </summary>
    public class DrawCardDto
    {
        [Required]
        public string PlayerId { get; set; }
    }
}