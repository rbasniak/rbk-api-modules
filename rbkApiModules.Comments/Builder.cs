using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
