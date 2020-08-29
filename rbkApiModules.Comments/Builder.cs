using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Infrastructure;
using System.Reflection;

namespace rbkApiModules.Comments
{
    public static class Builder
    {
        public static void AddRbkApiModulesComments(this IServiceCollection services)
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(ICommentsService)));

            AssemblyScanner
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CommentEntity.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}
