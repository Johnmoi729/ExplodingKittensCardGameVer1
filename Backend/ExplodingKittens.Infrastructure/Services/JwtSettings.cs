namespace ExplodingKittens.Infrastructure.Services
{
    /// <summary>
    /// Settings for JWT authentication
    /// </summary>
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}