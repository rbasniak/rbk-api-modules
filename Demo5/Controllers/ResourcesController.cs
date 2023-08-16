
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;

namespace Demo5;

[ApiController]
public class ResourcesController : BaseController
{
    public const string CURRENT_UNPROTECTED = "api/resources/current";
    public const string LEGACY_UNPROTECTED = "api/resources/legacy";
    public const string SHARED_UNPROTECTED = "api/resources/shared";
    public const string CURRENT_PROTECTED = "api/resources/current/protected";
    public const string LEGACY_PROTECTED = "api/resources/legacy/protected";
    public const string SHARED_PROTECTED = "api/resources/shared/protected";

    [Authorize]
    [HttpGet(CURRENT_UNPROTECTED)]
    public ActionResult<SimpleNamedEntity> GetResourceAsync()
    {
        return Ok();
    }

    [Authorize(AuthenticationSchemes = "Legacy")]
    [HttpGet(LEGACY_UNPROTECTED)]
    public ActionResult<SimpleNamedEntity> GetLegacyOnlyResourceAsync()
    {
        return Ok();
    }

    [Authorize(AuthenticationSchemes = "Legacy, Bearer")]
    [HttpGet(SHARED_UNPROTECTED)]
    public ActionResult<SimpleNamedEntity> GetSharedResourceAsync()
    {
        return Ok();
    }

    [RbkAuthorize("RESOURCE::READ")]
    [HttpGet(CURRENT_PROTECTED)]
    public ActionResult<SimpleNamedEntity> GetResourceWithClaimAsync()
    {
        return Ok();
    }

    [RbkAuthorize("RESOURCE::READ", AuthenticationSchemes = "Legacy")]
    [HttpGet(LEGACY_PROTECTED)]
    public ActionResult<SimpleNamedEntity> GetLegacyOnlyResourceWithClaimAsync()
    {
        return Ok();
    }

    [RbkAuthorize("RESOURCE::READ", AuthenticationSchemes = "Legacy, Bearer")]
    [HttpGet(SHARED_PROTECTED)]
    public ActionResult<SimpleNamedEntity> GetSharedResourceWithClaimAsync()
    {
        return Ok();
    }
}