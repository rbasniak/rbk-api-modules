using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using rbkApiModules.Commons.Core;
using Microsoft.AspNetCore.Authorization;
using LiteDB;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core.Logging;
using Serilog;

namespace Demo1.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoggingController : BaseController
{

    [AllowAnonymous()]
    [HttpGet("logging/test")]
    public ActionResult<string> TestDifferentLoggersAndLevels([FromServices] IDiagnosticsLogger diagnosticsLogger, [FromServices] IAnalyticsLogger analyticsLogger)
    {
        diagnosticsLogger.Logger.ForContext("id", "19").Verbose("[Diagnostics Logger] Verbose message");
        diagnosticsLogger.Logger.ForContext("id", "19").Debug("[Diagnostics Logger] Debug message");
        diagnosticsLogger.Logger.ForContext("id", "19").Information("[Diagnostics Logger] Information message");
        diagnosticsLogger.Logger.ForContext("id", "19").Warning("[Diagnostics Logger] Warning message");
        diagnosticsLogger.Logger.ForContext("id", "19").Error(new ApplicationException("test exception"), "[Diagnostics Logger] Error message");
        diagnosticsLogger.Logger.ForContext("id", "19").Fatal(new ApplicationException("test exception"), "[Diagnostics Logger] Fatal message");

        analyticsLogger.Logger.Verbose("[Analytics Logger] Verbose message");
        analyticsLogger.Logger.Debug("[Analytics Logger] Debug message");
        analyticsLogger.Logger.Information("[Analytics Logger] Information message");
        analyticsLogger.Logger.Warning("[Analytics Logger] Warning message");
        analyticsLogger.Logger.Error("[Analytics Logger] Error message");
        analyticsLogger.Logger.Fatal("[Analytics Logger] Fatal message");

        //Log.Logger.Verbose("[Internal Logger] Static Verbose message");
        //Log.Logger.Debug("[Internal Logger] Static Debug message");
        //Log.Logger.Information("[Internal Logger] Static Information message");
        //Log.Logger.Warning("[Internal Logger] Static Warning message");
        //Log.Logger.Error("[Internal Logger] Static Error message");
        //Log.Logger.Fatal("[Internal Logger] Static Fatal message");

        //Log.Logger.ForContext("Group", "Application").Verbose("[Static Logger w/ Context] Static Verbose message");
        //Log.Logger.ForContext("Group", "Application").Debug("[Static Logger w/ Context] Static Debug message");
        //Log.Logger.ForContext("Group", "Application").Information("[Static Logger w/ Context] Static Information message");
        //Log.Logger.ForContext("Group", "Application").Warning("[Static Logger w/ Context] Static Warning message");
        //Log.Logger.ForContext("Group", "Application").Error("[Static Logger w/ Context] Static Error message");
        //Log.Logger.ForContext("Group", "Application").Fatal("[Static Logger w/ Context] Static Fatal message");

        return Ok("Done");
    }
}