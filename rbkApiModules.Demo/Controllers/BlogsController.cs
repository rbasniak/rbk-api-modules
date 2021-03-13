using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Demo.Models;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities;
using rbkApiModules.Utilities.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class BlogsController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult> All([FromServices] DbContext context)
        {
            var blogs = await context.Set<Blog>().Include(x => x.Editors).Include(x => x.Posts).ToListAsync();

            return Ok("Done");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteBlog.Command { Id = id }));
        }
    }
}
