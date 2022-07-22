using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.MediatR.Core;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Excel;

[Route("api/[controller]")]
[ApiController]
public class ExcelController : BaseController
{

    /// <summary>
    /// Recebe dados e classes de formatação em JSON e devolve uma planilha em Excel
    /// </summary>
    [HttpPost("generate-tables")]
    [AllowAnonymous]
    public async Task<ActionResult<ExcelsDetails>> GenerateSpreadsheetTables(GenerateSpreadsheetTablesFromJson.Command data)
    {
        return HttpResponse(await Mediator.Send(data));
    }
}
