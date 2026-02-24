namespace rbkApiModules.Commons.Core;

public interface IBrokerPublisher
{
    Task PublishAsync(string topic, ReadOnlyMemory<byte> payload, IReadOnlyDictionary<string, object?>? headers, CancellationToken ct);
}
