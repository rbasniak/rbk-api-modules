using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Analytics.Relational.Core;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rbkApiModules.Analytics.Relational.SQLite
{
    [ExcludeFromCodeCoverage]
    public static class Builder
    {
        public static void AddSQLiteRbkApiAnalyticsModule(this IServiceCollection services, IConfiguration Configuration, string connectionString)
        {
            services.AddHostedService<SessionWriter>();

            services.AddTransient<ITransactionCounter, TransactionCounter>();

            services.AddTransient<DatabaseAnalyticsInterceptor>();

            services.Configure<RbkAnalyticsModuleOptions>(Configuration.GetSection(nameof(RbkAnalyticsModuleOptions)));

            services.AddDbContext<BaseAnalyticsContext, SQLiteAnalyticsContext>((scope, options) => options
                .UseSqlite(connectionString)
            );

            services.AddTransient<IAnalyticModuleStore, RelationalAnalyticStore>();
        }

        public static IApplicationBuilder UseSQLiteRbkApiAnalyticsModule(this IApplicationBuilder app, Action<AnalyticsModuleOptions> configureOptions)
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
                using (var context = scope.ServiceProvider.GetService<SQLiteAnalyticsContext>())
                {
                    context.Database.EnsureCreated();

                    if (context.Data.Count() == 0 && options.SeedSampleDatabase)
                    {
                        Seed.SeedDatabase(context);
                    }

                    int result = -1;
                    using (var connection = context.Database.GetDbConnection() as SqliteConnection)
                    {
                        connection.Open();

                        using (SqliteCommand command1 = connection.CreateCommand())
                        {
                            command1.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE name = 'Sessions'";

                            using (DbDataReader dataReader = command1.ExecuteReader())
                                if (dataReader.HasRows)
                                    while (dataReader.Read())
                                        result = dataReader.GetInt32(0);

                            var sessionTableExists = result > 0;

                            if (!sessionTableExists)
                            {
                                using (SqliteCommand command2 = connection.CreateCommand())
                                {
                                    command2.CommandText = @"CREATE TABLE Sessions ( 
                                                            Id       TEXT CONSTRAINT 'PK_Sessions' PRIMARY KEY
                                                                          NOT NULL,
                                                            Start    TEXT NOT NULL,
                                                            [End]    TEXT NOT NULL,
                                                            Username TEXT NULL,
                                                            Duration REAL NOT NULL 
                                                        );";
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
