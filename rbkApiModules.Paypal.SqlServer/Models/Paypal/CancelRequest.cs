using Newtonsoft.Json;

namespace rbkApiModules.Paypal.SqlServer
{
    public partial class CancelRequest
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
