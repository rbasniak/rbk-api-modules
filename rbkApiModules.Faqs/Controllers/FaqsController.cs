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

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteFaq.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }
    }
}
