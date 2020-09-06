using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Comments;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Api.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
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
            return HttpResponse<Dictionary<string, FormDefinition>> (await Mediator.Send(new GetUiDefinitions.Command()));
        }
    } 
}

