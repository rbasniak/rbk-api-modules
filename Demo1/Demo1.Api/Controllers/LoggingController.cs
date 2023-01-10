using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using rbkApiModules.Commons.Core;

namespace Demo1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingController : BaseController
{ 
    [HttpPost("log-exception-test")]
    public async Task<ActionResult<BaseResponse>> LogExceptionTest(LogExceptionTest.Command data)
    {
        Logger.LogTrace("request to {Method} {Url}", HttpContext.Request.Method.ToString(), HttpContext.Request.GetDisplayUrl());

        var response = await Mediator.Send(data);

        return Ok(response);
    }

    [HttpPost("scoped-log-test")]
    public async Task<ActionResult<BaseResponse>> ScopeLogTest(ScopeLogTest.Command data)
    {
        Logger.LogTrace("request to {Method} {Url}", HttpContext.Request.Method.ToString(), HttpContext.Request.GetDisplayUrl());

        var response = await Mediator.Send(data);

        Logger.LogTrace("request to {Method} {Url} handled with success", HttpContext.Request.Method.ToString(), HttpContext.Request.GetDisplayUrl());

        return Ok(response);
    }
}