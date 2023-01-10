using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Faqs.Core;

namespace rbkApiModules.Faqs.Relational;

public static class Builder
{
    public static void AddRbkRelationalFaqs(this IServiceCollection services)
    {
        services.AddRbkFaqs();

        services.AddScoped<IFaqsService, RelationalFaqsService>();
    }
}
