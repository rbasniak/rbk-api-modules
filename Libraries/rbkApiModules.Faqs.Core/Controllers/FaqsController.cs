using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Faqs.Core;

[Route("api/faqs")]
[ApiController]
public class FaqsController : BaseController
{
    [HttpGet]
    [Route("{tag}")]
    public async Task<ActionResult<FaqDetails[]>> All(string tag, CancellationToken cancellation)
    {
        return HttpResponse<FaqDetails[]>(await Mediator.Send(new GetFaqs.Command { Tag = tag }, cancellation));
    }

    [HttpPost]
    public async Task<ActionResult<FaqDetails>> Create(CreateFaq.Command data, CancellationToken cancellation)
    {
        return HttpResponse<FaqDetails>(await Mediator.Send(data, cancellation));
    }

    [HttpPut]
    public async Task<ActionResult<FaqDetails>> Create(UpdateFaq.Command data, CancellationToken cancellation)
    {
        return HttpResponse<FaqDetails>(await Mediator.Send(data, cancellation));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(new DeleteFaq.Command { Id = id }, cancellation));
    }
}
