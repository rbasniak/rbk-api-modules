using Serilog;
using Serilog.Events;

namespace Demo4;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(host => host.ConfigureKestrel(options => options.AddServerHeader = false))
                .UseSerilog((context, services, configuration) =>
                {
                    // var temp1 = context.Configuration.GetValue<string>("Log:SQLite");

                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()

                        .WriteTo.Debug(LogEventLevel.Debug)

                        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error);
                }, writeToProviders: true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application crashed");

            return 1;
        }
        finally
        {
        }
    }
}