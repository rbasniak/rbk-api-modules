using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using Demo1.Database.Read;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;
using System.Threading.Tasks;
using System.Linq;

namespace Demo1.Api.Controllers;

// All blogs/posts/author are centralized in this controller to make swagger clean
// This is not how this would behave in a real world application
[ApiController]
[Route("api/[controller]")]
public class CqrsPerformanceTextController : BaseController
{ 
    /// <summary>
    /// Adds X items do the performance tables
    /// </summary>
    [HttpPost("seed-tables")]
    public async Task<ActionResult<SeedPerformanceTables.Result>> SeedPerformanceTables(int size = 10000)
    {
        var response = await Mediator.Send(new SeedPerformanceTables.Request { Size = size });

        return HttpResponse(response);
    }

    /// <summary>
    /// Compares the reading speed of all items int the performance tables
    /// </summary>
    [HttpPost("test-read-performance")]
    public async Task<ActionResult<TestReadSpeed.Result>> TestReadSpeed()
    {
        var response = await Mediator.Send(new TestReadSpeed.Request());

        return HttpResponse(response);
    }

    [HttpPost("count")]
    public ActionResult<int> Count([FromServices] ReadDatabaseContext context)
    {
        return Ok(context.Set<PerformanceTest1>().Count());
    }
}