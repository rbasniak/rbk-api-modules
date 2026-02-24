using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace rbkApiModules.Commons.Core;

public sealed class RabbitMqPublisher : IBrokerPublisher, IAsyncDisposable
{
    private readonly BrokerOptions _options;
    private readonly ConnectionFactory _factory;

    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _sync = new object();

    public RabbitMqPublisher(IOptions<BrokerOptions> options)
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

    public Task PublishAsync(string topic, ReadOnlyMemory<byte> payload, IReadOnlyDictionary<string, object?>? headers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(topic);

        EnsureChannel();

        try
        {
            // idempot�ncia: mapear headers comuns e BasicProperties
            var props = _channel!.CreateBasicProperties();

            props.Persistent = true;

            props.Headers = new Dictionary<string, object?>(StringComparer.Ordinal);

            if (headers is not null && headers.Count > 0)
            {
                foreach (var kv in headers)
                {
                    props.Headers[kv.Key] = kv.Value;

                    if (kv.Key.Equals("message-id", StringComparison.Ordinal))
                    {
                        props.MessageId = TryGetString(kv.Value);
                    }
                    else if (kv.Key.Equals("correlation-id", StringComparison.Ordinal))
                    {
                        props.CorrelationId = TryGetString(kv.Value);
                    }
                }
            }

            // troca deve existir; se n�o existir, considere falha permanente
            try
            {
                _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
            }
            catch (OperationInterruptedException ex) when (IsPermanentDeclareFailure(ex))
            {
                throw new PermanentPublishException($"Exchange '{_options.Exchange}' not available.", ex);
            }

            _channel.BasicPublish(_options.Exchange, topic, mandatory: false, basicProperties: props, body: payload);

            // confirms com timeout + cancelamento cooperativo
            if (!WaitForConfirms(_channel, TimeSpan.FromSeconds(10), cancellationToken))
            {
                throw new TimeoutException("Publish confirm timeout.");
            }

            return Task.CompletedTask;
        }
        catch (OperationInterruptedException ex) when (IsPermanentPublishFailure(ex))
        {
            throw new PermanentPublishException(ex.Message, ex);
        }
        catch (AlreadyClosedException ex)
        {
            // fechar e deixar o decorator decidir retry
            SafeReset();
            throw;
        }
        catch (BrokerUnreachableException)
        {
            SafeReset();
            throw;
        }
        catch
        {
            throw;
        }
    }

    private void EnsureChannel()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return;
        }

        lock (_sync)
        {
            if (!(_connection is { IsOpen: true }))
            {
                _connection?.Dispose();
                _connection = _factory.CreateConnection();
            }

            if (!(_channel is { IsOpen: true }))
            {
                _channel?.Dispose();
                _channel = _connection.CreateModel();
                _channel.ConfirmSelect();
            }
        }
    }

    private static bool WaitForConfirms(IModel channel, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var end = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < end)
        {
            if (channel.WaitForConfirms(TimeSpan.FromMilliseconds(200)))
            {
                return true;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }

        return false;
    }

    private static bool IsPermanentDeclareFailure(OperationInterruptedException ex)
    {
        return ex.ShutdownReason != null && (ex.ShutdownReason.ReplyCode == 403 || ex.ShutdownReason.ReplyCode == 404);
    }

    private static bool IsPermanentPublishFailure(OperationInterruptedException ex)
    {
        if (ex.ShutdownReason is null)
        {
            return false;
        }

        var code = ex.ShutdownReason.ReplyCode;

        return code == 403 || code == 404 || code == 405 || code == 406;
    }

    private static string? TryGetString(object? x)
    {
        if (x is null)
        {
            return null;
        }

        if (x is string s)
        {
            return s;
        }

        if (x is byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }

        return x.ToString();
    }

    private void SafeReset()
    {
        try 
        { 
            _channel?.Close(); 
        } 
        catch 
        { 
        }
        
        try 
        { 
            _connection?.Close(); 
        } 
        catch 
        { 
        }

        try 
        { 
            _channel?.Dispose(); 
        } 
        catch 
        { 
        }

        try 
        { 
            _connection?.Dispose();
        } 
        catch 
        { 
        }
        
        _channel = null;
        
        _connection = null;
    }

    public ValueTask DisposeAsync()
    {
        SafeReset();
        
        return ValueTask.CompletedTask;
    }
}