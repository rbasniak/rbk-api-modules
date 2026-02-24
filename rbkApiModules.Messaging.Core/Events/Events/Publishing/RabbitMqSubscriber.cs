using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rbkApiModules.Commons.Core;

public sealed class RabbitMqSubscriber : IBrokerSubscriber, IDisposable
{
    private readonly BrokerOptions _options;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IModel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    public RabbitMqSubscriber(IOptions<BrokerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;

        _factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            RequestedHeartbeat = _options.Heartbeat,
            RequestedConnectionTimeout = _options.ConnectionTimeout,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };
    }

    public Task SubscribeAsync(string queue, IEnumerable<string> topics, Func<string, ReadOnlyMemory<byte>, IReadOnlyDictionary<string, object?>, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        // Create connection and channel if they don't exist or are closed
        if (_connection?.IsOpen != true)
        {
            _connection?.Dispose();
            _connection = _factory.CreateConnection();
        }
        
        if (_channel?.IsOpen != true)
        {
            _channel?.Dispose();
            _channel = _connection.CreateModel();
        }

        var args = new Dictionary<string, object?>
        {
            // Expect DLX preconfigured externally as options.DeadLetterExchange if desired
            // ["x-dead-letter-exchange"] = _options.DeadLetterExchange
        };

        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: args);

        foreach (var x in topics)
        {
            _channel.QueueBind(queue, _options.Exchange, x);
        }

        // backpressure
        _channel.BasicQos(prefetchSize: 0, prefetchCount: (ushort)(_options.PrefetchCount > 0 ? _options.PrefetchCount : 32), global: false);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += async (_, ea) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                IReadOnlyDictionary<string, object?> headers = ea.BasicProperties?.Headers is null
                    ? new Dictionary<string, object?>()
                    : new Dictionary<string, object?>(ea.BasicProperties.Headers);

                await handler(ea.RoutingKey, ea.Body, headers, cancellationToken);

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                // Do not requeue. Use DLX + retry queues for delays.
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: queue, autoAck: false, consumer: _consumer);

        // Block until cancellation
        var completion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        cancellationToken.Register(() => completion.TrySetResult(null));
        return completion.Task;
    }

    public void Dispose()
    {
        // Stop consumer first to prevent new messages from being processed
        try 
        { 
            _consumer = null; 
        } 
        catch 
        { 
        }

        // Close channel first to stop message processing
        try 
        { 
            if (_channel?.IsOpen == true)
            {
                _channel.Close();
            }
        } 
        catch 
        { 
        }
        
        // Close connection
        try 
        { 
            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }
        } 
        catch 
        { 
        }
        
        // Dispose channel
        try 
        { 
            _channel?.Dispose(); 
            _channel = null;
        } 
        catch 
        { 
        }
        
        // Dispose connection
        try 
        { 
            _connection?.Dispose(); 
            _connection = null;
        } 
        catch 
        { 
        }
    }
}
