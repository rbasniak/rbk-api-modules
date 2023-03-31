using Serilog.Events;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Exceptions;
using System.IO;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text.Json;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Sinks.PostgreSQL;
using Microsoft.Extensions.Configuration;

namespace Demo1.Api;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
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
            Log.Information("Starting API host");

            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(">>>>>>>>>>>>>>>>>>> " + msg));

                    // var temp1 = context.Configuration.GetValue<string>("Log:SQLite");

                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Logs");

                    IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
                    {
                        { "Timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                        { "Group", new SinglePropertyColumnWriter("Group", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
                        { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                        { "Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                        { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                        { "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                        { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                        // { "PropertiesTest", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                    };

                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()

                        .WriteTo.PostgreSQL(connectionString, 
                            restrictedToMinimumLevel: LogEventLevel.Verbose, 
                            tableName: "Logs", 
                            columnOptions: columnOptions, 
                            needAutoCreateTable: true, 
                            useCopy: true, 
                            queueLimit: 3000, 
                            batchSizeLimit: 1, 
                            period: new TimeSpan(0, 0, 1), 
                            formatProvider: null)

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
                }, writeToProviders: true)
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