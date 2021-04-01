using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Paypal.SqlServer;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    [Route("/[controller]")]
    [ApiController]
    public class PaypalController : BaseController
    {
        public PaypalController() : base()
        {
        }

        /// <summary>
        /// Trata um evento do tipo webhook vindo do Paypal
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var result = await Mediator.Send(new CreateWebhookEvent.Command 
            { 
                EventBody = HttpContext.Request.Body, 
                EventHeader = HttpContext.Request.Headers 
            });

            if (result.IsValid)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
            
        }
    }
}
