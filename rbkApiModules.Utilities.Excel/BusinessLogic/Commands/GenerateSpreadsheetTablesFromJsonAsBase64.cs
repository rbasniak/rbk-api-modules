﻿using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using FluentValidation;

namespace rbkApiModules.Utilities.Excel;

public class GenerateSpreadsheetTablesFromJsonAsBase64
{
    public class Command : IRequest<CommandResponse>, IExcelWorkbookModelJSonValidator
    {
        public ExcelWorkbookModel WorkbookModel { get; set; }
    }

    public class Handler : BaseCommandHandler<Command>
    {
        private readonly IExcelService _excelService;
        
        public Handler(IHttpContextAccessor httpContextAccessor, IExcelService excelService): base(httpContextAccessor)
        {
            _excelService = excelService;
        }

        protected override async Task<object> ExecuteAsync(Command request)
        {
            var result = _excelService.GenerateSpreadsheetTablesFromWorkbookModel(request.WorkbookModel);

            return result;
        }
    }
}
