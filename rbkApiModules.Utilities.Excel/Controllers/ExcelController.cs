using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.MediatR.Core;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Excel;

[IgnoreOnCodeGeneration]
[Route("api/[controller]")]
[ApiController]
public class ExcelController : BaseController
{

    /// <summary>
    /// Receives Data and formatting classes in JSON and returns an excel workbook in base64 format
    /// </summary>
    [HttpPost("generate-tables")]
    [AllowAnonymous]
    public async Task<ActionResult<SpreadsheetDetails>> GenerateSpreadsheetTables(GenerateSpreadsheetAsBase64.Command data)
    {
        return HttpResponse(await Mediator.Send(data));
    }

    /// <summary>
    /// Receives Data and formatting classes in JSON and returns an excel workbook in file format
    /// </summary>
    [HttpPost("generate-tables-file-output")]
    [AllowAnonymous]
    public async Task<ActionResult<SpreadsheetDetails>> GenerateSpreadsheetTablesAsFile(GenerateSpreadsheetAsStream.Command data)
    {
        var response = await Mediator.Send(data);
        if (response.IsValid)
        {
            var file = response.Result as FileData;
            return File(file.FileStream, file.ContentType, file.FileName);
        }

        return HttpResponse(response);
    }
}
