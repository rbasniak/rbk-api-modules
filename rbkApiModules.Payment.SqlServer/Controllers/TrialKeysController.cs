using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    [Route("api/trial-keys")]
    [ApiController]
    [AllowAnonymous]
    public class TrialKeysController : BaseController
    {
        /// <summary>
        /// Cria uma nova chave de degustação
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create(CreateTrialKey.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Associa o cliente e uma chave the degustação
        /// </summary>
        [HttpGet("activate/{trialKey}")]
        public async Task<ActionResult<TrialKeyDto.Details>> ActivateTrial(Guid trialKey)
        {
            return HttpResponse<TrialKeyDto.Details>(await Mediator.Send(new ActivateTrial.Command() { Key = trialKey }));
        }
    }
}
