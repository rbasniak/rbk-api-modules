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
    [HttpPost]
    public async Task<ActionResult<FileDto>> GenerateExcel(GenerateExcelFromJson.Command data)
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
