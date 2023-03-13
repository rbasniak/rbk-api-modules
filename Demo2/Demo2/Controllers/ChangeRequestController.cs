using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Demo2.Domain.Events;
using Microsoft.AspNetCore.Authorization;
using Demo2.Domain.Events.Repositories;
using Demo2.Domain;
using System.Diagnostics;
using Demo2.Domain.Events.MyImplementation.Database;

namespace Demo2.Controllers;

[ApiController]
[Route("api/change-request")]
public class ChangeRequestController: BaseController
{ 
    [HttpGet("seed/{amount}")]
    public async Task<ActionResult> Seed([FromServices] IChangeRequestRepository repository, [FromServices] RelationalContext rlContext, int amount)
    {
        try
        {
            var result = ChangeRequestSeeder.Generate(rlContext, amount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(ex.ToBetterString());
        }
    }
}
