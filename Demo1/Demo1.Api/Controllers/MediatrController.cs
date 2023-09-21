using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using rbkApiModules.Commons.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Demo1.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MediatRController : BaseController
{ 
    [HttpPost("pipeline-validation-test")]
    public async Task<ActionResult> PipelineValidationTest(PipelineValidationTest.Request data)
    {
        var response = await Mediator.Send(data);

        return Ok(response);
    }
}