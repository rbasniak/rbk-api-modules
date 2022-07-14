using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Utilities.Excel;

public class GenerateExcelFromJson
{
    public class Command : IRequest<CommandResponse>
    {
        public string ExcelJson { get; set; }
    }

    public class Validator: AbstractValidator<Command>  
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Validator(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
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
            var result = _excelService.GenerateExcel(request.ExcelJson);

            return result;
        }
    }
}
