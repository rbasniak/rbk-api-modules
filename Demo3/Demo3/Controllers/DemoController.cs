using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Identity.Core;
using System.ComponentModel;
using rbkApiModules.Commons.Core.Logging;

namespace Demo3;

[ApiController]
[IgnoreOnCodeGeneration]
[Route("api/[controller]")]
public class DemoController : BaseController
{
    [AllowAnonymous]
    [HttpGet("anonymous")]
    public ActionResult<Response> GetAnonymous()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an open endpoint",
        });
    }

    [Authorize]
    [HttpGet("authorized/low-privilegies")]
    public ActionResult<Response> GetAuthorizedWithLowPrivilegies()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an authenticated endpoint with low privilegies",
        });
    }

    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    [HttpGet("authorized/high-privilegies1")]
    public ActionResult<Response> GetAuthorizedWithHighPrivilegies1()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an authenticated endpoint with high privilegies",
        });
    }    
    
    [RbkAuthorize("NON_EXISTENT_CLAIM")]
    [HttpGet("authorized/high-privilegies2")]
    public ActionResult<Response> GetAuthorizedWithHighPrivilegies2()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "You should not have access to this",
        });
    }

    [AllowAnonymous()]
    [HttpGet("localization/resource-from-application/with-description")]
    public ActionResult<Response> GetLocalizedResourceFromApplicationWithDescription([FromServices] ILocalizationService localization)
    {
        return Ok(localization.LocalizeString(ApplicationMessages.Common.ValueWithDescription));
    }

    [AllowAnonymous()]
    [HttpGet("localization/resource-from-application/without-description")]
    public ActionResult<Response> GetLocalizedResourceFromApplicationWithoutDescription([FromServices] ILocalizationService localization)
    {
        return Ok(localization.LocalizeString(ApplicationMessages.Common.ValueWithoutDescription));
    }

    [AllowAnonymous()]
    [HttpGet("localization/localized-string")]
    public ActionResult<Response> GetLocalizedResource([FromServices] ILocalizationService localization)
    {
        return Ok(localization.LocalizeString(AuthenticationMessages.Validations.InvalidCredentials));
    }
}

public class Response
{
    public required DateTime Timestamp { get; set; }
    public required string Message { get; set; }
}

public class ApplicationMessages : ILocalizedResource
{
    public enum Common
    {
        [Description("Message from description attribute")] ValueWithDescription,
        ValueWithoutDescription,
    }
}