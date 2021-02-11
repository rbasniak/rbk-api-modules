namespace rbkApiModules.Paypal.SqlServer
{
    public class PaypalSettings
    {
        public string ClientUrl { get; set; }
        public string PaypalClientID { get; set; }
        public string PaypalSecret { get; set; }
        public string WebhookId { get; set; }
    }
}
