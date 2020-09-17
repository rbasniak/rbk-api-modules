using Microsoft.AspNetCore.Builder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Logging.Core
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiLogModule()
        {

        }

        public static IApplicationBuilder UseRbkApiLogModule(this IApplicationBuilder app)
        {
            // app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);

            return app;
        }
    }
}
