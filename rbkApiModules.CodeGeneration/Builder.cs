using Microsoft.AspNetCore.Builder;
using System;

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
