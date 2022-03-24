using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : BaseController
    {
        /// <summary>
        /// Lista os planos disponíveis
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PlanDto.Details[]>> All()
        {
            return HttpResponse<PlanDto.Details[]>(await Mediator.Send(new GetAllPlans.Command()));
        }

        /// <summary>
        /// Lista os planos disponíveis e ativos
        /// </summary>
        [HttpGet]
        [Route("active")]
        public async Task<ActionResult<PlanDto.Details[]>> AllActive()
        {
            return HttpResponse<PlanDto.Details[]>(await Mediator.Send(new GetAllActivePlans.Command()));
        }

        /// <summary>
        /// Cria um novo planos
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PlanDto.Details>> Create(CreatePlan.Command data)
        {
            return HttpResponse<PlanDto.Details>(await Mediator.Send(data));
        }

        /// <summary>
        /// Atualiza os dados de um planos
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<PlanDto.Details>> Update(UpdatePlan.Command data)
        {
            return HttpResponse<PlanDto.Details>(await Mediator.Send(data));
        }

        /// <summary>
        /// Apaga um planos
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeletePlan.Command { Id = id }));
        }
    }
}
