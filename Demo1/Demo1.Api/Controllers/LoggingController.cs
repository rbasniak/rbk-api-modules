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
    public ActionResult<string> TestDifferentLoggersAndLevels([FromServices] IInternalLogger internalLogger)
    {
        internalLogger.Logger.Verbose("[Internal Logger] Verbose message");
        internalLogger.Logger.Debug("[Internal Logger] Debug message");
        internalLogger.Logger.Information("[Internal Logger] Information message");
        internalLogger.Logger.Warning("[Internal Logger] Warning message");
        internalLogger.Logger.Error("[Internal Logger] Error message");
        internalLogger.Logger.Fatal("[Internal Logger] Fatal message");

        Log.Logger.Verbose("[Internal Logger] Static Verbose message");
        Log.Logger.Debug("[Internal Logger] Static Debug message");
        Log.Logger.Information("[Internal Logger] Static Information message");
        Log.Logger.Warning("[Internal Logger] Static Warning message");
        Log.Logger.Error("[Internal Logger] Static Error message");
        Log.Logger.Fatal("[Internal Logger] Static Fatal message");

        Log.Logger.ForContext("Group", "Application").Verbose("[Internal Logger] Static Verbose message");
        Log.Logger.ForContext("Group", "Application").Debug("[Internal Logger] Static Debug message");
        Log.Logger.ForContext("Group", "Application").Information("[Internal Logger] Static Information message");
        Log.Logger.ForContext("Group", "Application").Warning("[Internal Logger] Static Warning message");
        Log.Logger.ForContext("Group", "Application").Error("[Internal Logger] Static Error message");
        Log.Logger.ForContext("Group", "Application").Fatal("[Internal Logger] Static Fatal message");

        return Ok("Done");
    }
}