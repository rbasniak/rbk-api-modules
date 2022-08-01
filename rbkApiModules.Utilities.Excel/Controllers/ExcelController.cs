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
    /// Receives Data and formatting classes in JSON and returns an excel workbook in base64 format
    /// </summary>
    [HttpPost("generate-tables")]
    public async Task<ActionResult<ExcelsDetails>> GenerateSpreadsheetTables(GenerateSpreadsheetTablesFromJsonAsBase64.Command data)
    {
        return HttpResponse(await Mediator.Send(data));
    }

    /// <summary>
    /// Receives Data and formatting classes in JSON and returns an excel workbook in file format
    /// </summary>
    [HttpPost("generate-tables-file-output")]
    public async Task<ActionResult<ExcelsDetails>> GenerateSpreadsheetTablesAsFile(GenerateSpreadsheetTablesFromJsonAsFile.Command data)
    {
        var response = await Mediator.Send(data);
        if (response.IsValid)
        {
            var file = response.Result as FileDto;
            return File(file.FileStream, file.ContentType, file.FileName);
        }

        return HttpResponse(response);
    }
}
