using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Infrastructure.Utilities.MongoDB
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool IsSSL { get; set; }
    }
}
