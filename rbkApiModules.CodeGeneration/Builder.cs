using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Utilities.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace rbkApiModules.CodeGeneration
{
    public static class Builder
    { 
        public static IApplicationBuilder UseRbkCodeGenerationModule(this IApplicationBuilder app, Action<CodeGenerationModuleOptions> configureOptions)
        {
            configureOptions(CodeGenerationModuleOptions.Instance);

            return app;
        }
    }
}
