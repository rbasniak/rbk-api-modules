using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Demo2.Domain.Events;
using Microsoft.AspNetCore.Authorization;

namespace Demo2.Controllers;

[ApiController]
[Route("api/change-request")]
public class ChangeRequestController: BaseController
{
    [AllowAnonymous]
    [HttpPost("create-by-general-user")]
    public async Task<ActionResult> CreateByGeneralUser(CreateChangeRequestByGeneralUser.Request request)
    {
        var response = await Mediator.Send(request);

        return HttpResponse(response);
    }

    [AllowAnonymous]
    [HttpPost("fics/add")]
    public async Task<ActionResult> AddFic(AddFicToChangeRequest.Request request)
    {
        var response = await Mediator.Send(request);

        return HttpResponse(response);
    }

    [AllowAnonymous]
    [HttpPost("fics/remove")]
    public async Task<ActionResult> RemoveFic(RemoveFicToChangeRequest.Request request)
    {
        var response = await Mediator.Send(request);

        return HttpResponse(response);
    }
}
