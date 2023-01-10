using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;

namespace rbkApiModules.Commons.Core.UiDefinitions;

[IgnoreOnCodeGeneration]
[Route("api/ui-definitions")]
[ApiController]
public class UiDefinitionsController : BaseController
{

    [AllowAnonymous]
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

