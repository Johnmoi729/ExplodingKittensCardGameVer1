using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for playing a special combo
    /// </summary>
    public class PlayComboDto
    {
        [Required]
        public string PlayerId { get; set; }

        [Required]
        public List<string> CardIds { get; set; }

        [Required]
        public string TargetPlayerId { get; set; }
    }
}