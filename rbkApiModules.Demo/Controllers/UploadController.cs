using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Demo.Controllers.Abstract;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities.Extensions;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UploadController: BaseUploadController
    {
        [HttpPut]
        [Route("instance/confirm")]
        public async Task<ActionResult> ConfirmInstance()
        {
            var result = await ProcessUploadedFile(0);

            if (!result.Success)
            {
                return BadRequest(new string[] { result.Error });
            }

            if (!result.FormData.GetResults().TryGetValue("data", out StringValues rawData))
            {
                return BadRequest(new string[] { "Não foi possível identificar os dados da requisição" });
            }

            return Ok();
        }
    }
}
