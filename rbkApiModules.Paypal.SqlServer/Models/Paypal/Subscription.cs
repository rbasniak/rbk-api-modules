using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace rbkApiModules.Paypal.SqlServer
{
    public partial class Subscription
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_change_note")]
        public string StatusChangeNote { get; set; }

        [JsonProperty("status_update_time")]
        public string StatusUpdateTime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("shipping_amount")]
        public ShippingAmount ShippingAmount { get; set; }

        [JsonProperty("subscriber")]
        public Subscriber Subscriber { get; set; }

        [JsonProperty("billing_info")]
        public BillingInfo BillingInfo { get; set; }

        [JsonProperty("create_time")]
        public string CreateTime { get; set; }

        [JsonProperty("update_time")]
        public string UpdateTime { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public partial class BillingInfo
    {
        [JsonProperty("outstanding_balance")]
        public ShippingAmount OutstandingBalance { get; set; }

        [JsonProperty("cycle_executions")]
        public List<CycleExecution> CycleExecutions { get; set; }

        [JsonProperty("last_payment")]
        public LastPayment LastPayment { get; set; }

        [JsonProperty("next_billing_time")]
        public string NextBillingTime { get; set; }

        [JsonProperty("final_payment_time")]
        public string FinalPaymentTime { get; set; }

        [JsonProperty("failed_payments_count")]
        public long FailedPaymentsCount { get; set; }
    }

    public partial class LastPayment
    {
        [JsonProperty("amount")]
        public ShippingAmount Amount { get; set; }

        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }
    }

    public partial class CycleExecution
    {
        [JsonProperty("tenure_type")]
        public string TenureType { get; set; }

        [JsonProperty("sequence")]
        public string Sequence { get; set; }

        [JsonProperty("cycles_completed")]
        public string CyclesCompleted { get; set; }

        [JsonProperty("cycles_remaining")]
        public string CyclesRemaining { get; set; }

        [JsonProperty("total_cycles")]
        public string TotalCycles { get; set; }

        [JsonProperty("current_pricing_scheme_version")]
        public string CurrentPricingSchemeVersion { get; set; }
    }

    public partial class ShippingAmount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    public partial class ShippingAddress
    {
        [JsonProperty("address")]
        public Address Address { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("address_line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("admin_area_2")]
        public string AdminArea2 { get; set; }

        [JsonProperty("admin_area_1")]
        public string AdminArea1 { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}
