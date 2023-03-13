using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Demo2.Domain.Events;
using Microsoft.AspNetCore.Authorization;
using Demo2.Domain.Events.Repositories;
using Demo2.Domain;
using System.Diagnostics;
using Demo2.Domain.Events.MyImplementation.Database;

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

    [AllowAnonymous]
    [HttpGet("seed/{amount}")]
    public async Task<ActionResult> Seed([FromServices] IChangeRequestRepository repository, [FromServices] RelationalContext rlContext, int amount)
    {
        try
        {
            var result = ChangeRequestGenerator.Generate(rlContext, amount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(ex.ToBetterString());
        }
    }
}
