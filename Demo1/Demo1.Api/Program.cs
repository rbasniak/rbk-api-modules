using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Formatting.Compact;
using Serilog.Events;
using System;
using Serilog.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using rbkApiModules.Commons.Core.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;
using System.IO;
using Serilog.Formatting.Json;
using System.Diagnostics;
using Serilog.Exceptions;

namespace Demo1.Api;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .Enrich.WithExceptionDetails()

            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error)

            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "startup-.log"),
                restrictedToMinimumLevel: LogEventLevel.Debug,
                fileSizeLimitBytes: 1024 * 1024,
                shared: true,
                rollOnFileSizeLimit: true,
                retainedFileTimeLimit: TimeSpan.FromDays(10),
                rollingInterval: RollingInterval.Day)

            .WriteTo.Debug(LogEventLevel.Debug)

            .CreateBootstrapLogger();

        try
        {
            Host.CreateDefaultBuilder(args)
                .UseSerilog(configureLogger: (context, services, configuration) =>
                {
                    Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(">>>>>>>>>>>>>>>>>>> " + msg));

                    //// var temp1 = context.Configuration.GetValue<string>("Log:SQLite");

                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Logs");

                    var analyticsColumnOptions = new Dictionary<string, ColumnWriterBase>
                    {
                        { "Timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },    
                        { "Path", new SinglePropertyColumnWriter("Path", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                        { "PathBase", new SinglePropertyColumnWriter("PathBase", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                        { "Method", new SinglePropertyColumnWriter("Method", PropertyWriteMethod.Raw, NpgsqlDbType.Text, "l") },
                        { "Response", new SinglePropertyColumnWriter("Response", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) },
                        { "Username", new SinglePropertyColumnWriter("Username", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                        { "IpAddress", new SinglePropertyColumnWriter("IpAddress", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                        { "ConnectionId", new SinglePropertyColumnWriter("ConnectionId", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                    };

                    var diagnosticsColumnOptions = new Dictionary<string, ColumnWriterBase>
                    {
                        { "Timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                        { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                        { "Properties", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                        { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                        { "Template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                        { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) }
                    };

                    configuration
                    // .ReadFrom.Configuration(context.Configuration)
                    // .ReadFrom.Services(services)
                    // .Enrich.FromLogContext()
                    // .Enrich.WithExceptionDetails()

                        .MinimumLevel.Verbose()
                        .WriteTo.Logger(configuration => configuration
                            .MinimumLevel.Debug()
                            .Filter.ByIncludingOnly(x => x.Properties.ContainsKey("SourceContext") && x.Properties["SourceContext"].ToString() == "\"Diagnostics\"")
                            .Enrich.WithExceptionDetails()
                            .WriteTo.File(new RenderedCompactJsonFormatter(), "Logs\\log_diagnostics.json",
                                restrictedToMinimumLevel: LogEventLevel.Verbose,
                                shared: true,
                                flushToDiskInterval: TimeSpan.FromSeconds(1))
                            .WriteTo.PostgreSQL(connectionString,
                                restrictedToMinimumLevel: LogEventLevel.Verbose,
                                tableName: "Diagnostics",
                                columnOptions: diagnosticsColumnOptions,
                                needAutoCreateTable: true,
                                useCopy: true,
                                queueLimit: 50,
                                batchSizeLimit: 5,
                                period: new TimeSpan(0, 0, 5),
                                formatProvider: null)
                        )
                        .WriteTo.Logger(configuration => configuration
                            .MinimumLevel.Information()
                            .Filter.ByIncludingOnly(x => x.Properties.ContainsKey("SourceContext") && x.Properties["SourceContext"].ToString() == "\"Analytics\"")
                            .Enrich.With(services.GetRequiredService<HttpContextEnricher>())
                            .WriteTo.File(new RenderedCompactJsonFormatter(), "Logs\\log_analytics.json",
                                restrictedToMinimumLevel: LogEventLevel.Verbose,
                                shared: true,
                                flushToDiskInterval: TimeSpan.FromSeconds(1))
                            .WriteTo.PostgreSQL(connectionString,
                                restrictedToMinimumLevel: LogEventLevel.Verbose,
                                tableName: "Analytics",
                                columnOptions: analyticsColumnOptions,
                                needAutoCreateTable: true,
                                useCopy: true,
                                queueLimit: 50,
                                batchSizeLimit: 25,
                                period: new TimeSpan(0, 0, 30),
                                formatProvider: null)
                        )

                        .WriteTo.Debug(LogEventLevel.Debug)

                        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error)

                        .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "log-.log"),
                            restrictedToMinimumLevel: LogEventLevel.Verbose,
                            fileSizeLimitBytes: 1024 * 1024,
                            shared: true,
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day)

                        .WriteTo.File(new JsonFormatter(renderMessage: true), Path.Combine(Environment.CurrentDirectory, "Logs", "log-.json"),
                            restrictedToMinimumLevel: LogEventLevel.Verbose,
                            fileSizeLimitBytes: 1024 * 1024,
                            shared: true,
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day)

                        .WriteTo.LiteDB(Path.Combine(Environment.CurrentDirectory, "Logs", "log.lite"),
                            restrictedToMinimumLevel: LogEventLevel.Debug)

                        .WriteTo.SQLite(Path.Combine(Environment.CurrentDirectory, "Logs", "log.db"),
                            restrictedToMinimumLevel: LogEventLevel.Debug,
                            storeTimestampInUtc: true,
                            batchSize: 1)

                        .WriteTo.Seq("http://localhost:5341/")
                        ;

                    //configuration
                    //    .MinimumLevel.Verbose()
                    //    .WriteTo.Logger(configuration => configuration
                    //        .MinimumLevel.Debug()
                    //        .Filter.ByIncludingOnly(x => x.Properties.ContainsKey("SourceContext") && x.Properties["SourceContext"].ToString() == "\"Diagnostics\"")
                    //        .Enrich.With(enricher)
                    //        .WriteTo.File(new RenderedCompactJsonFormatter(), "Logs\\log_diagnostics.json",
                    //            restrictedToMinimumLevel: LogEventLevel.Verbose,
                    //            shared: true,
                    //            flushToDiskInterval: TimeSpan.FromSeconds(1))
                    //    )
                    //    .WriteTo.Logger(configuration => configuration
                    //        .MinimumLevel.Information()
                    //        .Filter.ByIncludingOnly(x => x.Properties.ContainsKey("SourceContext") && x.Properties["SourceContext"].ToString() == "\"Analytics\"")
                    //        .WriteTo.File(new RenderedCompactJsonFormatter(), "Logs\\log_analytics.json",
                    //            restrictedToMinimumLevel: LogEventLevel.Verbose,
                    //            shared: true,
                    //            flushToDiskInterval: TimeSpan.FromSeconds(1))
                    //    )
                    //    .WriteTo.Console();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();

            Log.Information("Stopping API host");

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

