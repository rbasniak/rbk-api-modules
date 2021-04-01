using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Database.StateMachine;
using System.Linq;

namespace rbkApiModules.Demo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            //// Create the Serilog logger, and configure the sinks
            //Serilog.Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Information()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            //    // .Enrich.FromLogContext()
            //    // .Enrich.With<ReleaseNumberEnricher>()
            //    .WriteTo.Console()
            //    .WriteTo.File(new CompactJsonFormatter(), "d:\\serilog.json")
            //    .CreateLogger();

            try
            {
                // var test = new SimpleNamedEntity { Id = "1", Name = "Oi" };
                // Serilog.Log.Logger.Information("Starting host {@SimpleNamedEntity}", test);

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<DatabaseContext>();
                        context.Database.Migrate();

                        if (!context.States.Any())
                        {
                            Seed.CreateEntities(context);
                        }

                        context.SaveChanges();
                    }
                    catch
                    {
                        // Serilog.Log.Logger.Fatal(ex, "An error occurred while seeding the database.");
                    }
                }

                host.Run();

                return 0;
            }
            catch
            {
                // Serilog.Log.Logger.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                // Serilog.Log.Logger.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
