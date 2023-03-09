using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : BaseController
{
    [HttpPost("all")]
    public async Task<ActionResult<NotificationDetails[]>> GetNotifications(GetNotifications.Request data)
    {
        return HttpResponse<NotificationDetails[]>(await Mediator.Send(data));
    }

    [HttpPut("update")]
    public async Task<ActionResult<string[]>> UpdateNotifications(UpdateNotificationStatus.Request data)
    {
        return HttpResponse<string[]>(await Mediator.Send(data));
    }

    [HttpPost("delete")]
    public async Task<ActionResult<string[]>> DeleteNotifications(DeleteNotification.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse<string[]>(result);
    }
}
