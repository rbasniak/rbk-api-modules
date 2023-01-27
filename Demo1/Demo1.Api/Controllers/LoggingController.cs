using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using rbkApiModules.Commons.Core;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using LiteDB;

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

    [AllowAnonymous]
    [HttpPost("log-levels-test")]
    public ActionResult LogLevelsTest([FromServices] ILogger<LoggingController> logger)
    {
        Logger.LogDebug("DEBUG DATA 1");
        Logger.LogInformation("INFORMATION DATA 1");
        Logger.LogWarning("WARNING DATA 1");
        Logger.LogError("ERROR DATA 1");

        try
        {
            throw new ArgumentNullException("parameter");
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "FATAL DATA 1");
        }

        logger.LogDebug("DEBUG DATA 2");
        logger.LogInformation("INFORMATION DATA 2");
        logger.LogWarning("WARNING DATA 2");
        logger.LogError("ERROR DATA 2");

        try
        {
            throw new ArgumentNullException("parameter");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "FATAL DATA 2");
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("read-logs")]
    public ActionResult ReadLogsTest()
    {
        //using (var db = new LiteDatabase(Path.Combine(Environment.CurrentDirectory, "Logs", "log.lt")))
        //{
        //    var temp1 = db.GetCollection("log").Count();

        //    var temp2 = db.GetCollection("log").Query().Where(x => x["SourceContext"].AsString.StartsWith("rbk")).ToList();
        //}

        Mediator.Publish(new EventHappened());

        return Ok();
    }
}