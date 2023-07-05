
using Demo3.Commands;
using Demo3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace Demo3;
[AllowAnonymous]
[Route("api/projects")]
[ApiController]
public class ProjectsController : BaseController
{

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectDto.Details>> CreateAsync(CreateProject.Request data, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(data, cancellationToken);

        return HttpResponse<ProjectDto.Details>(result);
    }
}