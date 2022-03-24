using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Logs.Core;
using rbkApiModules.Utilities;
using System;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuxiliariesModulesController : BaseController
    {
        [HttpGet]
        [Route("test-analytics")]
        public ActionResult TestAnalytics([FromServices] IAnalyticModuleStore store)
        {
            try
            {
                store.StoreData(new AnalyticsEntry()
                {
                    Action = "Action",
                    Area = "Area",
                    Domain = "Domain",
                    Duration = 1,
                    Identity = "Identity",
                    IpAddress = "IpAddress",
                    Method = "Method",
                    Path = "Path",
                    RequestSize = 1,
                    ResponseSize = 1,
                    Response = 1,
                    Timestamp = DateTime.Now,
                    TotalTransactionTime = 1,
                    TransactionCount = 1,
                    UserAgent = "UserAgent",
                    Username = "UserName",
                    Version = "Version",
                    WasCached = false
                });
            }
            catch (Exception ex)
            {
                return Ok(ex.ToBetterString());
            }

            return Ok("Done");
        }

        [HttpGet]
        [Route("test-diagnostics")]
        public ActionResult TestDiagnostics([FromServices] IDiagnosticsModuleStore store)
        {
            try
            {
                store.StoreData(new DiagnosticsEntry()
                {
                    ApplicationArea = "ApplicationArea",
                    ApplicationLayer = "ApplicationLayer",
                    ApplicationVersion = "ApplicationVersion",
                    ClientBrowser = "ClientBrowser",
                    Username = "Username",
                    ClientDevice = "ClientDevice",
                    ClientOperatingSystem = "ClientOperatingSystem",
                    ClientOperatingSystemVersion = "ClientOperatingSystemVersion",
                    ClientUserAgent = "ClientUserAgent",
                    DatabaseExceptions = "DatabaseExceptions",
                    Domain = "Domain",
                    ExceptionMessage = "ExceptionMessage",
                    ExceptionSource = "ExceptionSource",
                    ExtraData = "ExtraData",
                    InputData = "InputData",
                    RequestId = "RequestId",
                    StackTrace = "StackTrace",
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Ok(ex.ToBetterString());
            }
            return Ok("Done");
        }

        [HttpGet]
        [Route("test-logs")]
        public ActionResult TestLogs([FromServices] ILogsModuleStore store)
        {
            try
            {
                store.StoreData(new LogEntry()
                {
                    ApplicationArea = "ApplicationArea",
                    ApplicationLayer = "ApplicationLayer",
                    ApplicationVersion = "ApplicationVersion",
                    Domain = "Domain",
                    Enviroment = "Enviroment",
                    EnviromentVersion = "EnviromentVersion",
                    InputData = "InputData",
                    Level = LogLevel.Warning,
                    MachineName = "MachineName",
                    Message = "Message",
                    Source = "Source",
                    Timestamp = DateTime.Now,
                    Username = "Username"
                });
            }
            catch (Exception ex)
            {
                return Ok(ex.ToBetterString());
            }
            return Ok("Done");
        }

    }
}
