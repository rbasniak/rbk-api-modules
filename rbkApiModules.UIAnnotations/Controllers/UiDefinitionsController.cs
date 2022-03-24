using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.UIAnnotations
{
    [IgnoreOnCodeGeneration]
    [Route("api/ui-definitions")]
    [ApiController]
    public class UiDefinitionsController : BaseController
    {

        /// <summary>
        /// Retorna a lista de todas as definições de UI
        /// </summary>
        [AllowAnonymous]
        [NgxsDatabaseStore(StoreType.Readonly)]
        [HttpGet]
        public async Task<ActionResult<object>> All()
        {
            var response = await Mediator.Send(new GetUiDefinitions.Command());

            var temp = new JsonResult(response.Result, new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return temp;
        }
    } 
}

