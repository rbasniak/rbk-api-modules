using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core;

public sealed class ResilientBrokerPublisher : IBrokerPublisher
{
    private readonly IBrokerPublisher _inner;
    private readonly ILogger<ResilientBrokerPublisher> _logger;

    public ResilientBrokerPublisher(IBrokerPublisher inner, ILogger<ResilientBrokerPublisher> logger)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(logger);

        _inner = inner;
        _logger = logger;
    }

    public async Task PublishAsync(string topic, ReadOnlyMemory<byte> payload, IReadOnlyDictionary<string, object?>? headers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(topic);

        var attempt = 0;

        while (true)
        {
            attempt++;

            try
            {
                await _inner.PublishAsync(topic, payload, headers, cancellationToken);
                return;
            }
            catch (PermanentPublishException)
            {
                throw;
            }
            catch (Exception ex) when (IsPermanent(ex))
            {
                throw new PermanentPublishException(ex.Message, ex);
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                if (attempt >= 5)
                {
                    _logger.LogError(ex, "Transient publish failure after {Attempt} attempts on topic {Topic}.", attempt, topic);
                    
                    throw;
                }

                var delay = BackoffWithJitter(attempt, TimeSpan.FromMilliseconds(200));
                
                _logger.LogWarning(ex, "Transient publish failure. Retry {Attempt} in {Delay}ms on topic {Topic}.", attempt, delay.TotalMilliseconds, topic);
                
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(Exception x)
    {
        if (x is TimeoutException) 
        { 
            return true; 
        }

        if (x is IOException) 
        { 
            return true; 
        }

        if (x is RabbitMQ.Client.Exceptions.BrokerUnreachableException) 
        { 
            return true; 
        }

        if (x is RabbitMQ.Client.Exceptions.AlreadyClosedException) 
        { 
            return true; 
        }

        if (x is RabbitMQ.Client.Exceptions.OperationInterruptedException op &&
            op.ShutdownReason != null &&
            (op.ShutdownReason.ReplyCode == 320 || op.ShutdownReason.ReplyCode == 541)) 
        { 
            return true; 
        } // connection forced / internal error

        if (x.InnerException is not null && IsTransient(x.InnerException)) 
        { 
            return true; 
        }
        
        return false;
    }

    private static bool IsPermanent(Exception x)
    {
        if (x is PermanentPublishException) 
        { 
            return true; 
        }

        if (x is UnauthorizedAccessException) 
        { 
            return true; 
        }

        if (x is RabbitMQ.Client.Exceptions.OperationInterruptedException op &&
            op.ShutdownReason != null &&
            (op.ShutdownReason.ReplyCode == 404 || op.ShutdownReason.ReplyCode == 403 || op.ShutdownReason.ReplyCode == 405 || op.ShutdownReason.ReplyCode == 406)) 
        { 
            return true; 
        }
        
        return false;
    }

    private static TimeSpan BackoffWithJitter(int attempt, TimeSpan seed)
    {
        if (attempt < 1) 
        { 
            attempt = 1; 
        }
        
        var max = Math.Min(seed.TotalMilliseconds * Math.Pow(2, attempt - 1), 30000d);

        var waitMs = Random.Shared.NextDouble() * max;
        
        return TimeSpan.FromMilliseconds(waitMs);
    }
}

public sealed class PermanentPublishException : Exception
{
    public PermanentPublishException(string message, Exception? inner = null) : base(message, inner) { }
}