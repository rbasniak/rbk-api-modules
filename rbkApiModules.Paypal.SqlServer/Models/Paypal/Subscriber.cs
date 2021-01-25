using Newtonsoft.Json;

namespace rbkApiModules.Paypal.SqlServer
{
    public partial class Subscriber
    {
        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }
}
