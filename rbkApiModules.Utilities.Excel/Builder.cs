using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

namespace rbkApiModules.Utilities.Excel;

public static class Builder
{
    public static void AddRbkApiExcelModule(this IServiceCollection services)
    {
        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(IExcelService)));

        AssemblyScanner
            .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(GenerateSpreadsheetAsBase64.Command)))
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
    }
}
