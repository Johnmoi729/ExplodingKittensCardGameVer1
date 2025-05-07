namespace ExplodingKittens.Domain.Constants
{
    /// <summary>
    /// Constants used in the Exploding Kittens game
    /// </summary>
    public static class GameConstants
    {
        // Player limits
        public const int MinPlayers = 2;
        public const int MaxPlayers = 5;

        // Card counts
        public const int InitialHandSize = 7;
        public const int InitialDefuseCount = 1;
        public const int ExplodingKittenCount = 4;
        public const int DefuseCount = 6;
        public const int AttackCount = 4;
        public const int SkipCount = 4;
        public const int ShuffleCount = 4;
        public const int SeeFutureCount = 5;
        public const int FavorCount = 4;
        public const int CatCardCount = 4; // Per type

        // Card effects
        public const int SeeFutureCardCount = 3;

        // Game statuses
        public const string Waiting = "waiting";
        public const string InProgress = "in_progress";
        public const string Completed = "completed";
    }
}