using System.Threading.Tasks;

namespace rbkApiModules.Paypal.SqlServer
{
    public interface IPaypalActions
    {
        public Task OnWebhookEventReceived(WebhookEventResponse response);
    }
}
