using Serilog.Events;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Exceptions;

namespace Demo1.Api;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341", LogEventLevel.Information)
            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "log.txt"), LogEventLevel.Debug, fileSizeLimitBytes: 1024 * 1024, shared: true, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day)
            .WriteTo.File(new JsonFormatter(renderMessage: true), Path.Combine(Environment.CurrentDirectory, "Logs", "log.json"), LogEventLevel.Debug, fileSizeLimitBytes: 1024 * 1024, shared: true, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
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
            Log.Fatal(ex, "Host terminated unexpectedly");

            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}