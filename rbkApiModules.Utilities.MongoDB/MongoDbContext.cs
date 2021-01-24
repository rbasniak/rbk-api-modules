using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace rbkApiModules.Utilities.MongoDB
{
    public class MongoDbContext
    {
        private readonly MongoDbConfiguration _options;

        public MongoDbContext(IOptionsSnapshot<MongoDbConfiguration> snapshotOptionsAccessor)
        {
            try
            {
                _options = snapshotOptionsAccessor.Value;

                var settings = MongoClientSettings.FromUrl(new MongoUrl(_options.ConnectionString));

                if (_options.IsSSL)
                {
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                }

                var mongoClient = new MongoClient(settings);

                Database = mongoClient.GetDatabase(_options.Database);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to the server.", ex);
            }
        }

        public IMongoDatabase Database { get; set; }
    }
}
