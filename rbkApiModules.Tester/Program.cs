using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreApiTemplate.Auditing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rbkApiModules.Authentication;
using rbkApiModules.Tester.Database;
using rbkApiModules.Tester.Models;

namespace rbkApiModules.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context1 = services.GetRequiredService<DatabaseContext>();
                    context1.Database.Migrate();

                    var user = context1.Set<User>().SingleOrDefault(x => x.Username == "admn");

                    if (user == null)
                    {
                        user = new User("admn", "123123", false, new Client("Administrador", DateTime.Now));
                        context1.Set<User>().Add(user);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim("SAMPLE_CLAIM", "Exemplo de Controle de Acesso")
                    };

                    foreach (var claim in claims)
                    {
                        if (!context1.Set<Claim>().Any(x => x.Name == claim.Name))
                        {
                            context1.Set<Claim>().Add(claim);

                            user.AddClaim(claim, ClaimAcessType.Allow);
                        }
                    }

                    context1.SaveChanges();

                    var context2 = services.GetRequiredService<AuditingContext>();
                    context2.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
