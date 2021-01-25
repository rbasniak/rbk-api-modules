namespace rbkApiModules.Paypal.SqlServer
{
    public interface IPaypalActions
    {
        public void OnWebhookEventReceived(WebhookEventResponse response);
    }
}
