﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Diagnostics.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.Analytics.SqlServer
{
    [ExcludeFromCodeCoverage]
    public static class Builder
    {
        public static void AddSqlServerRbkApiAnalyticsModule(this IServiceCollection services, IConfiguration Configuration, string connectionString)
        {
            services.AddTransient<ITransactionCounter, TransactionCounter>();

            services.AddTransient<DatabaseAnalyticsInterceptor>();

            services.Configure<RbkAnalyticsModuleOptions>(Configuration.GetSection(nameof(RbkAnalyticsModuleOptions)));

            services.AddDbContext<SqlServerAnalyticsContext>((scope, options) => options
                .UseSqlServer(connectionString)
                .AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
            //.EnableDetailedErrors()
            //.EnableSensitiveDataLogging()
            );

            services.AddTransient<IAnalyticModuleStore, SqlServerAnalyticStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiAnalyticsModule(this IApplicationBuilder app, Action<AnalyticsModuleOptions> configureOptions)
        {
            var options = new AnalyticsModuleOptions();
            configureOptions(options);

            app.UseMiddleware<AnalyticsModuleMiddleware>(options);

            if (options.SessionIdleLimit > 0)
            {
                app.UseMiddleware<SessionAnalyticsMiddleware>(options);
            }

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerAnalyticsContext>())
                {
                    context.Database.EnsureCreated();

                    if (context.Data.Count() == 0 && options.SeedSampleDatabase)
                    {
                        Seed.SeedDatabase(context);
                    }

                    int result = -1;
                    using (var connection = context.Database.GetDbConnection() as SqlConnection)
                    {
                        connection.Open();

                        using (SqlCommand command1 = connection.CreateCommand())
                        {
                            command1.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Sessions'";

                            using (DbDataReader dataReader = command1.ExecuteReader())
                                if (dataReader.HasRows)
                                    while (dataReader.Read())
                                        result = dataReader.GetInt32(0);

                            var sessionTableExists = result > 0;

                            if (!sessionTableExists)
                            {
                                using (SqlCommand command2 = connection.CreateCommand())
                                {
                                    command2.CommandText = @"CREATE TABLE [dbo].[Sessions] (
                                                    [Id]                   UNIQUEIDENTIFIER NOT NULL,
                                                    [Start]                DATETIME2 (7)    NOT NULL,
                                                    [End]                  DATETIME2 (7)    NOT NULL,
                                                    [Username]             NVARCHAR (128)   NULL,
                                                    [Duration]             Float (7)        NOT NULL,
                                                )";
                                    command2.ExecuteNonQuery();
                                }
                            }
                        }

                        connection.Close();
                    }
                }
            }

            return app;
        }
    }
}
