using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

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
