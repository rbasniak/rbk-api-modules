//using FluentValidation;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System.Diagnostics;
//using System.Threading;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

//namespace rbkApiModules.Commons.Core;
//public class Dispatcher
//{
//    private readonly IServiceProvider _serviceProvider;
//    private readonly IHttpContextAccessor _httpContextAccessor;

//    public Dispatcher(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
//    {
//        _serviceProvider = serviceProvider;
//        _httpContextAccessor = httpContextAccessor;
//    }

//    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken)
//    {
//        var commandType = request.GetType();
//        var logger = _serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(commandType)) as ILogger;

//        var commandTypeName = commandType.FullName.Split(".").Last().Replace("+", ".");

//        logger.LogDebug("Executing command {CommandType}", commandTypeName);

//        var stopwatch = Stopwatch.StartNew();

//        try
//        {
//            DetectAuthenticatedUser(request);

//            await ValidateAsync(logger, commandType, request, cancellationToken);

//            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
//            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
//            var response = await handler.HandleAsync((dynamic)request, cancellationToken);
            
//            stopwatch.Stop();
//            logger.LogInformation("Command {CommandType} executed successfully in {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);
            
//            return response;
//        }
//        catch (Exception ex) when (ex is not InternalValidationException)
//        {
//            stopwatch.Stop();
//            logger.LogError(ex, "Error executing command {CommandType} after {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);
//            throw new UnexpectedInternalException("Error during validation of the request", ex);
//        }
//    }

//    private void  DetectAuthenticatedUser(object request)
//    {
//        if (request is IAuthenticatedRequest authenticatedRequest)
//        {
//            var user = _httpContextAccessor.HttpContext.User;

//            if (user.Identity.IsAuthenticated)
//            {
//                var claims = user.Claims
//                    .Where(x => x.Type == JwtClaimIdentifiers.Roles)
//                    .Select(x => x.Value)
//                    .ToArray();

//                authenticatedRequest.SetIdentity(_httpContextAccessor.GetTenant(), _httpContextAccessor.GetUsername(), claims);
//            }
//        }
//    }

//    public async Task SendAsync(ICommand request, CancellationToken cancellationToken)
//    {
//        var commandType = request.GetType();
//        var logger = _serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(commandType)) as ILogger;

//        var commandTypeName = commandType.FullName.Split(".").Last().Replace("+", ".");

//        logger.LogDebug("Executing command {CommandType}", commandTypeName);

//        var stopwatch = Stopwatch.StartNew();

//        try
//        {
//            DetectAuthenticatedUser(request);

//            await ValidateAsync(logger, commandType, request, cancellationToken);

//            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
//            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
//            await handler.HandleAsync((dynamic)request, cancellationToken);

//            stopwatch.Stop();
//            logger.LogInformation("Command {CommandType} executed successfully in {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);
//        }
//        catch (Exception ex) when (ex is not InternalValidationException)
//        {
//            stopwatch.Stop();
//            logger.LogError(ex, "Error executing command {CommandType} after {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);
//            throw new UnexpectedInternalException("Error during validation of the request", ex);
//        }
//    }

//    private async Task ValidateAsync(ILogger logger, Type commandType, object request, CancellationToken cancellationToken)
//    {
//        var commandTypeName = commandType.FullName.Split(".").Last().Replace("+", ".");

//        var validatorBaseType = typeof(AbstractValidator<>).MakeGenericType(commandType);

//        var validator = _serviceProvider.GetService(validatorBaseType);

//        if (validator is IValidator concreteValidator)
//        {
//            logger.LogDebug("Validating command {CommandType}", commandTypeName);

//            var context = new ValidationContext<object>(request);
//            var result = await concreteValidator.ValidateAsync(context, cancellationToken);
//            if (!result.IsValid)
//            {
//                var errorSummary = new Dictionary<string, string[]>();
//                foreach (var error in result.Errors)
//                {
//                    if (error.ErrorMessage == "ignore me!")
//                    {
//                        continue;
//                    }

//                    if (!errorSummary.ContainsKey(error.PropertyName))
//                    {
//                        errorSummary[error.PropertyName] = [error.ErrorMessage];
//                    }
//                    else
//                    {
//                        errorSummary[error.PropertyName] = errorSummary[error.PropertyName].Append(error.ErrorMessage).ToArray();
//                    }
//                }

//                logger.LogWarning("Command validation failed for {CommandType}: {Errors}", commandTypeName, errorSummary);
//                throw new InternalValidationException(errorSummary);
//            }
//        }
//    }

//    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken)
//    {
//        var commandType = request.GetType();
//        var logger = _serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(commandType)) as ILogger;

//        var commandTypeName = commandType.FullName.Split(".").Last().Replace("+", ".");

//        logger.LogDebug("Executing command {CommandType}", commandTypeName);

//        var stopwatch = Stopwatch.StartNew();

//        try
//        {
//            DetectAuthenticatedUser(request);

//            await ValidateAsync(logger, commandType, request, cancellationToken);

//            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(commandType, typeof(TResponse));
//            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
//            var response = await handler.HandleAsync((dynamic)request, cancellationToken);

//            stopwatch.Stop();
//            logger.LogInformation("Command {CommandType} executed successfully in {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);

//            return response;
//        }
//        catch (Exception ex) when (ex is not InternalValidationException)
//        {
//            stopwatch.Stop();
//            logger.LogError(ex, "Error executing command {CommandType} after {ElapsedMilliseconds}ms", commandTypeName, stopwatch.ElapsedMilliseconds);
//            throw new UnexpectedInternalException("Error during validation of the request", ex);
//        }
//    }

//    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
//        where TNotification : INotification
//    {
//        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

//        foreach (var handler in handlers)
//        {
//            try
//            {
//                await handler.HandleAsync(notification, cancellationToken);
//            }
//            catch (Exception ex)
//            {
//                // Log and swallow to prevent crashing the request
//                Console.WriteLine($"[Dispatcher] Notification handler failed: {ex.Message}");
//            }
//        }
//    }
//}


//public interface ICommand { }
//public interface ICommandHandler<TCommand>
//    where TCommand : ICommand
//{
//    Task HandleAsync(TCommand request, CancellationToken cancellationToken = default);
//}

//public interface ICommand<TResponse> { }
//public interface ICommandHandler<TCommand, TResponse>
//    where TCommand : ICommand<TResponse>
//{
//    Task<TResponse> HandleAsync(TCommand request, CancellationToken cancellationToken = default);
//}

//public interface IQuery<TResponse> { }
//public interface IQueryHandler<TQuery, TResponse>
//    where TQuery : IQuery<TResponse>
//{
//    Task<TResponse> HandleAsync(TQuery request, CancellationToken cancellationToken = default);
//}

//public interface INotification { }
//public interface INotificationHandler<TNotification>
//    where TNotification : INotification
//{
//    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
//}
