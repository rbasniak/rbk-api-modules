using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace rbkApiModules.SharedUI
{
    [ExcludeFromCodeCoverage]
    [IgnoreOnCodeGeneration]
    [ApplicationArea("shared-ui")]
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
}
