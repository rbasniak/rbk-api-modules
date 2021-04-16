using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Paypal.SqlServer
{
    [ExcludeFromCodeCoverage]
    public partial class CancelRequest
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
