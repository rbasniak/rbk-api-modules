using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Comments;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseController
    {
        /// <summary>
        /// Retorna uma lista de notificações
        /// </summary>
        [HttpPost("all")]
        public async Task<ActionResult<NotificationDto[]>> GetNotifications(GetNotifications.Command data)
        {
            return HttpResponse<NotificationDto[]>(await Mediator.Send(data));
        }

        /// <summary>
        /// Atualiza o status de um array de notificações
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<string[]>> UpdateNotifications(UpdateNotificationStatus.Command data)
        {
            return HttpResponse<string[]>(await Mediator.Send(data));
        }

        /// <summary>
        /// Deleta um array de notificações
        /// </summary>
        [HttpPost("delete")]
        public async Task<ActionResult<string[]>> DeleteNotifications(DeleteNotification.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<string[]>(result);
        }
    }
}
