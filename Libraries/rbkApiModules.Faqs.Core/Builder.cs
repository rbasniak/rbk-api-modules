using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace rbkApiModules.Faqs.Core;

public static class Builder
{
    public static void AddRbkFaqs(this IServiceCollection services)
    {
        AssemblyScanner
            .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CreateFaq.Request)))
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
    }
}
