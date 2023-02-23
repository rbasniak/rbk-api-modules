using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using rbkApiModules.Commons.Core;

namespace Demo1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediatRController : BaseController
{ 
    [HttpPost("pipeline-validation-test")]
    public async Task<ActionResult<BaseResponse>> PipelineValidationTest(PipelineValidationTest.Command data)
    {
        var response = await Mediator.Send(data);

        return Ok(response);
    }
}