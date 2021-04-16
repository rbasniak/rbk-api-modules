using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Paypal.SqlServer
{
    [ExcludeFromCodeCoverage]
    public class PaypalWebhookSignatureVerification
    {
        [JsonProperty("auth_algo")]
        public string AuthAlgo { get; set; }

        [JsonProperty("cert_url")]
        public string CertUrl { get; set; }

        [JsonProperty("transmission_id")]
        public string TransmissionId { get; set; }

        [JsonProperty("transmission_sig")]
        public string TransmissionSig { get; set; }

        [JsonProperty("transmission_time")]
        public string TransmissionTime { get; set; }

        [JsonProperty("webhook_id")]
        public string WebhookId { get; set; }

        [JsonProperty("webhook_event")]
        public string WebhookEvent { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class PaypalWebhookEventResponse
    {
        [JsonProperty("verification_status")]
        public string VerificationStatus { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public partial class WebhookEventResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("create_time")]
        public string CreateTime { get; set; }

        [JsonProperty("resource_type")]
        public string ResourceType { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("resource")]
        public Resource Resource { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public partial class Resource
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }

        [JsonProperty("billing_agreement_id")]
        public string BillingAgreementId { get; set; }

        [JsonProperty("subscriber")]
        public Subscriber Subscriber { get; set; }
    }
}
