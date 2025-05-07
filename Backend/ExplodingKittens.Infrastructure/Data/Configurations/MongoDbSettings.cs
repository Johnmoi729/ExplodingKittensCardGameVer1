namespace ExplodingKittens.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Settings for MongoDB connection
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}