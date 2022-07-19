using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Faqs
{
    [Route("api/faqs")]
    [ApiController]
    public class FaqsController : BaseController
    {

        [HttpGet]
        [Route("{tag}")]
        public async Task<ActionResult<FaqDetails[]>> All(string tag)
        {
            return HttpResponse<FaqDetails[]>(await Mediator.Send(new GetFaqs.Command { Tag = tag }));
        }

        [HttpPost]
        public async Task<ActionResult<FaqDetails>> Create(CreateFaq.Command data)
        {
            return HttpResponse<FaqDetails>(await Mediator.Send(data));
        }

        [HttpPut]
        public async Task<ActionResult<FaqDetails>> Create(UpdateFaq.Command data)
        {
            return HttpResponse<FaqDetails>(await Mediator.Send(data));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteFaq.Command { Id = id }));
        }
    }
}
