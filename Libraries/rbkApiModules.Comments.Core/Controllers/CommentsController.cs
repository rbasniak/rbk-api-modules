using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : BaseController
{

    [HttpPost]
    public async Task<ActionResult<TreeNode[]>> CreateComment([FromBody] CommentEntity.Request data, CancellationToken cancellation)
    {
        return HttpResponse<TreeNode[]>(await Mediator.Send(data, cancellation));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TreeNode[]>> AllComments(Guid id, CancellationToken cancellation)
    {
        return HttpResponse<TreeNode[]>(await Mediator.Send(new GetComments.Request { EntityId = id }, cancellation));
    }
}
