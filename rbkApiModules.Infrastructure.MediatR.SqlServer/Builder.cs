using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiMediatRModuleSqlServer(this IServiceCollection services)
        {
            services.AddTransient<ICommonDatabaseValidations, CommonDatabaseValidations>();
        }
    }
}
