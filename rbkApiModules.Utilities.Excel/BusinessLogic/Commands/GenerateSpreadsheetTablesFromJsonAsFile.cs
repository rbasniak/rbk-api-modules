using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Utilities.Excel;

public class GenerateSpreadsheetTablesFromJsonAsFile
{
    public class Command : IRequest<CommandResponse>
    {
        public ExcelWorkbookModel WorkbookModel { get; set; }
    }

    public class Handler : BaseCommandHandler<Command>
    {
        private readonly IExcelService _excelService;
        
        public Handler(IHttpContextAccessor httpContextAccessor, IExcelService excelService)
            : base(httpContextAccessor)
        {
            _excelService = excelService;
        }

        protected override async Task<object> ExecuteAsync(Command request)
        {
            var result = _excelService.GenerateSpreadsheetTablesFromWorkbookModelAsFile(request.WorkbookModel);

            return result;
        }
    }
}
