using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Paypal.SqlServer
{
    [ExcludeFromCodeCoverage]
    public partial class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }
}
