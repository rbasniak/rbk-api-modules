using Testcontainers.RabbitMq;
using TUnit.Core.Interfaces;

namespace rbkApiModules.Commons.Testing;

public sealed class RabbitFixture : IAsyncInitializer, IAsyncDisposable
{
    private RabbitMqContainer? _container;
    public RabbitMqContainer Container => _container ?? throw new InvalidOperationException("RabbitMQ não inicializado");
    public Uri ManagementBase { get; private set; } = null!;
    public string Amqp => Container.GetConnectionString();

    public string User => "guest";
    public string Pass => "guest";

    public async Task InitializeAsync()
    {
        _container = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.13-management")
            .WithUsername(User).WithPassword(Pass)
            .Build();

        await _container.StartAsync();

        ManagementBase = new Uri($"http://{_container.Hostname}:{_container.GetMappedPublicPort(15672)}/api/");
    }

    public ValueTask DisposeAsync() => _container?.DisposeAsync() ?? ValueTask.CompletedTask;
}