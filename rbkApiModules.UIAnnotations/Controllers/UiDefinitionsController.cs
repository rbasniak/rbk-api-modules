using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rbkApiModules.UIAnnotations
{
    [Route("api/ui-definitions")]
    [ApiController]
    public class UiDefinitionsController : BaseController
    {

        /// <summary>
        /// Retorna a lista de todas as definições de UI
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, FormDefinition>>> All()
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

