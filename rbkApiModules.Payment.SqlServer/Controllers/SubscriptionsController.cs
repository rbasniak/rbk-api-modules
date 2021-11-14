using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : BaseController
    {
        /// <summary>
        /// Cria uma nova assinatura
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SubscriptionDto.Details>> Create(CreateSubscription.Command data)
        {
            return HttpResponse<SubscriptionDto.Details>(await Mediator.Send(data));
        }

        /// <summary>
        /// Cancela uma assinatura
        /// </summary>
        [HttpPost]
        [Route("cancel")]
        public async Task<ActionResult> Cancel(CancelSubscription.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }
    }
}
