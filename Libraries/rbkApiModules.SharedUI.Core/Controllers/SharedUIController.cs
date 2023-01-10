using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;

namespace rbkApiModules.SharedUI.Core;

[IgnoreOnCodeGeneration]
[AllowAnonymous]
[ApiController]
[Route("api/shared-ui")]
public class SharedUIController : BaseController
{
    [HttpGet]
    [Route("menu")]
    public ActionResult<MenuInfo> GetFilterData()
    {
        return Ok(new MenuInfo
        {
            UseAnalytics = SharedUIModuleOptions._useAnalytics,
            UseDiagnostics = SharedUIModuleOptions._useDiagnostics,
            CustomRoutes = SharedUIModuleOptions._customRoutes.ToArray()
        });
    }

    [HttpPost]
    [Route("auth")]
    public async Task<ActionResult<JwtResponse>> Login(UserLogin.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }
}
