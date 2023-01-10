using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using Demo1.DataTransfer;
using rbkApiModules.Commons.Core;
using Demo1.BusinessLogic.Queries;

namespace Demo1.Api.Controllers;

// All blogs/posts/author are centralized in this controller to make swagger clean
// This is not how this would behave in a real world application
[ApiController]
[Route("api/[controller]")]
public class BlogsController : BaseController
{ 
    [HttpGet("blogs")]
    public async Task<ActionResult<Models.Read.Blog[]>> AllBlogs()
    {
        var response = await Mediator.Send(new GetAllBlogs.Command());

        return HttpResponse(response);
    }

    [HttpGet("authors")]
    public async Task<ActionResult<SimpleNamedEntity[]>> AllAuthors()
    {
        var response = await Mediator.Send(new GetAllAuthors.Command());

        return HttpResponse<SimpleNamedEntity[]>(response);
    }

    [HttpGet("posts")]
    public async Task<ActionResult<PostDetails[]>> AllPosts()
    {
        var response = await Mediator.Send(new GetAllPosts.Command());

        return HttpResponse<PostDetails[]>(response);
    }

    [HttpPost("posts")]
    public async Task<ActionResult<PostDetails>> CreatePost(CreatePost.Command data)
    {
        var response = await Mediator.Send(data);

        return HttpResponse<PostDetails>(response);
    }

    [HttpPut("posts")]
    public async Task<ActionResult<PostDetails>> UpdatePost(UpdatePost.Command data)
    {
        var response = await Mediator.Send(data);

        return HttpResponse<PostDetails>(response);
    }
}