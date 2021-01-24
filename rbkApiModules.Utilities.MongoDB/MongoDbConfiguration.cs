namespace rbkApiModules.Utilities.MongoDB
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool IsSSL { get; set; }
    }
}
