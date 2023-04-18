using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core;
using System.Reflection;

namespace rbkApiModules.Faqs.Core;

public static class Builder
{
    public static void AddRbkFaqs(this IServiceCollection services)
    {
        services.RegisterFluentValidators(Assembly.GetAssembly(typeof(CreateFaq.Request)));
    }
}
