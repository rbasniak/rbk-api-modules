using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Authentication;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Database.StateMachine;

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
                var test = new SimpleNamedEntity { Id = "1", Name = "Oi" };

                // Serilog.Log.Logger.Information("Starting host {@SimpleNamedEntity}", test);

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<DatabaseContext>();
                        context.Database.Migrate();

                        var user = context.Set<BaseUser>().FirstOrDefault();

                        if (user == null)
                        {
                            var user1 = new ClientUser("client", "123123", false, new Client("John Doe", DateTime.Now));
                            context.Set<BaseUser>().Add(user1);

                            var user2 = new ManagerUser("manager", "123123", false, new Manager("Jane Doe"));
                            context.Set<BaseUser>().Add(user2);
                        }

                        var claims = new List<Claim>
                        {
                            new Claim("SAMPLE_CLAIM", "Exemplo de Controle de Acesso", "client"),
                            new Claim("CAN_BUY", "Pode realizar compras", "client"),
                            new Claim("SAMPLE_CLAIM", "Exemplo de Controle de Acesso", "manager"),
                            new Claim("CAN_VIEW_REPORTS", "Pode visualizar relatórios", "manager")
                        };

                        foreach (var claim in claims)
                        {
                            if (!context.Set<Claim>().Any(x => x.Name == claim.Name && x.AuthenticationGroup == claim.AuthenticationGroup))
                            {
                                context.Set<Claim>().Add(claim);
                            }
                        }

                        context.SaveChanges();

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
