using Newtonsoft.Json;
using System.Collections.Generic;

namespace rbkApiModules.Paypal.SqlServer
{
    public partial class Error
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("debug_id")]
        public string DebugId { get; set; }

        [JsonProperty("details")]
        public List<Detail> Details { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public partial class Detail
    {
        [JsonProperty("issue")]
        public string Issue { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
