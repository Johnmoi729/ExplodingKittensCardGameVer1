using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for playing a card
    /// </summary>
    public class PlayCardDto
    {
        [Required]
        public string PlayerId { get; set; }

        [Required]
        public string CardId { get; set; }

        public string TargetPlayerId { get; set; }
    }
}