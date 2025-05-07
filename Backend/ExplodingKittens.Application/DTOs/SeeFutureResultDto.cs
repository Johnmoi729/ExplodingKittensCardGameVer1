using System.Collections.Generic;

namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for the result of a See Future card
    /// </summary>
    public class SeeFutureResultDto
    {
        public List<CardDto> TopCards { get; set; }
    }
}