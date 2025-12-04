using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace rbkApiModules.Commons.Testing;

public sealed class PostgresFixture : IAsyncInitializer, IAsyncDisposable
{
    public PostgreSqlContainer Container { get; private set; } = default!;
    public string AdminConnectionString =>
        new Npgsql.NpgsqlConnectionStringBuilder(Container.GetConnectionString()) { Database = "postgres" }.ToString();

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithUsername("postgres").WithPassword("postgres")
            .Build();
        await Container.StartAsync();
    }

    public ValueTask DisposeAsync() => Container?.DisposeAsync() ?? ValueTask.CompletedTask;
}
