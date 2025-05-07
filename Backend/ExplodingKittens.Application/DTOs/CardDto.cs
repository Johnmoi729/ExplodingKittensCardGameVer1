namespace ExplodingKittens.Application.DTOs
{
    /// <summary>
    /// Data transfer object for a card
    /// </summary>
    public class CardDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Effect { get; set; }
        public string ImageUrl { get; set; }
    }
}