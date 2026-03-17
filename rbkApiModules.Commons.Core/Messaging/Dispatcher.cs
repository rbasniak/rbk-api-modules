using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core.Helpers;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Concurrent;

namespace rbkApiModules.Commons.Core;

public interface IDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) where TResponse : BaseResponse;

    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification;
}

public class Dispatcher(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
    : IDispatcher
{
	private static readonly ConcurrentDictionary<(Type RequestType, Type ResponseType), Type?> HandlerContractCache = new();

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        where TResponse : BaseResponse
    {
        var commandType = request.GetType();
        var commandTypeName = commandType.FullName.Split(".").Last().Replace("+", ".");
        var logger = serviceProvider.GetService(typeof(ILogger<>).MakeGenericType(commandType)) as ILogger;

        logger.LogInformation("Executing command {CommandType}", commandTypeName);

        var sw = Stopwatch.StartNew();
        BaseResponse? result = null;

        using var root = EventsTracing.ActivitySource.StartActivity("dispatcher.request", ActivityKind.Internal);
        root?.SetTag("dispatcher.request.type", commandTypeName);

        try
        {
            // ---- identity propagation span
            using (var idAct = EventsTracing.ActivitySource.StartActivity("dispatcher.identity", ActivityKind.Internal))
            {
                var user = httpContextAccessor?.HttpContext?.User;
                idAct?.SetTag("auth.present", user != null);
                idAct?.SetTag("auth.is_authenticated", user?.Identity?.IsAuthenticated == true);
                try
                {
                    PropagateAuthenticatedUser(ref request);
                    idAct?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    idAct?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    idAct?.AddEvent(new ActivityEvent("exception",
                        tags: new ActivityTagsCollection {
                        { "exception.type", ex.GetType().FullName },
                        { "exception.message", ex.Message }
                        }));
                    throw;
                }
            }

            // ---- validation (has its own nested spans in ValidateAsync)
            var validationResult = await ValidateAsync(logger, commandType, request!, cancellationToken);
            if (validationResult.Count > 0)
            {
                result = CommandResponseFactory.CreateFailed(request, new ValidationProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = validationResult
                });
                return (TResponse)result;
            }

            // ---- reflection/DI resolution span
            object? handler;
            List<object> behaviors;
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            using (var resolveAct = EventsTracing.ActivitySource.StartActivity("dispatcher.resolve", ActivityKind.Internal))
            {
                resolveAct?.SetTag("request.type", commandTypeName);
                resolveAct?.SetTag("handler.contract", handlerType.FullName);

                var cached = TryGetOrResolveHandlerContractType<TResponse>(commandType, resolveAct);
                if (cached is not null)
                {
                    handlerType = cached;
                    resolveAct?.SetTag("handler.contract", handlerType.FullName);
                }

                handler = cached is null ? null : serviceProvider.GetService(handlerType);
                resolveAct?.SetTag("handler.found", handler is not null);

                behaviors = serviceProvider.GetServices(typeof(IPipelineBehavior<,>)
                                    .MakeGenericType(request.GetType(), typeof(TResponse)))
                                    .Cast<object>()
                                    .ToList();

                resolveAct?.SetTag("behaviors.count", behaviors.Count);
                if (behaviors.Count > 0)
                {
                    resolveAct?.AddEvent(new ActivityEvent("behaviors.list",
                        tags: new ActivityTagsCollection {
                        { "behaviors", string.Join(",", behaviors.Select(b => b.GetType().Name)) }
                        }));
                }
                if (handler is null)
                {
                    resolveAct?.SetStatus(ActivityStatusCode.Error, "Handler not found");
                }
                else
                {
                    resolveAct?.SetStatus(ActivityStatusCode.Ok);
                }
            }

            if (handler == null)
            {
                result = CommandResponseFactory.CreateFailed(request, new ProblemDetails
                {
                    Title = "Handler Not Found",
                    Detail = $"No handler registered for request type {request.GetType().FullName}",
                    Status = StatusCodes.Status500InternalServerError
                });
                return (TResponse)result;
            }

            // ---- handler delegate with span + reflection tags
            Func<Task<TResponse>> handle = async () =>
            {
                using var handlerAct = EventsTracing.ActivitySource.StartActivity("dispatcher.handler", ActivityKind.Internal);
                handlerAct?.SetTag("dispatcher.request.type", commandTypeName);

                var swHandle = Stopwatch.StartNew();
                logger.LogDebug("Starting to handle command {CommandType}", commandTypeName);

                dynamic dynHandler = serviceProvider.GetRequiredService(handlerType);
                var implType = ((object)dynHandler).GetType();
                handlerAct?.SetTag("handler.impl", implType.FullName);
                var mi = implType.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
                handlerAct?.SetTag("handler.method", mi?.ToString());

                try
                {
                    var response = await dynHandler.HandleAsync((dynamic)request, cancellationToken);
                    handlerAct?.SetStatus(ActivityStatusCode.Ok);
                    return response;
                }
                catch (Exception ex)
                {
                    handlerAct?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    handlerAct?.AddEvent(new ActivityEvent("exception",
                        tags: new ActivityTagsCollection {
                        { "exception.type", ex.GetType().FullName },
                        { "exception.message", ex.Message }
                        }));
                    throw;
                }
                finally
                {
                    swHandle.Stop();
                    handlerAct?.SetTag("handler.elapsed_ms", swHandle.ElapsedMilliseconds);
                    logger.LogDebug("Command {CommandType} handled in {ElapsedMilliseconds}ms", commandTypeName, swHandle.ElapsedMilliseconds);
                }
            };

            // ---- pipeline behaviors, each wrapped with its own span
            foreach (var behavior in behaviors.Reverse<object>())
            {
                var next = handle;
                handle = async () =>
                {
                    using var behAct = EventsTracing.ActivitySource.StartActivity("dispatcher.behavior.invoke", ActivityKind.Internal);
                    var bt = behavior.GetType();
                    behAct?.SetTag("behavior.type", bt.FullName);

                    var swB = Stopwatch.StartNew();
                    try
                    {
                        var method = bt.GetMethod("Handle");
                        behAct?.SetTag("behavior.method", method?.ToString());

                        var response = await (Task<TResponse>)method!.Invoke(behavior, new object[] { request!, cancellationToken, next })!;
                        behAct?.SetStatus(ActivityStatusCode.Ok);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        behAct?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        behAct?.AddEvent(new ActivityEvent("exception",
                            tags: new ActivityTagsCollection {
                            { "exception.type", ex.GetType().FullName },
                            { "exception.message", ex.Message }
                            }));
                        throw;
                    }
                    finally
                    {
                        swB.Stop();
                        behAct?.SetTag("behavior.elapsed_ms", swB.ElapsedMilliseconds);
                        logger.LogDebug("Behavior {Behavior} finished in {duration}ms", bt.Name, swB.ElapsedMilliseconds);
                    }
                };
            }

            result = await handle();
            return (TResponse)result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing command {CommandType}", commandTypeName);

            var message = TestingEnvironmentChecker.IsTestingEnvironment ? ex.ToString() : ex.Message;
            result = CommandResponseFactory.CreateFailed(request, new ProblemDetails
            {
                Title = "Command Execution Failed",
                Detail = message,
                Status = StatusCodes.Status500InternalServerError
            });
            return (TResponse)result;
        }
        finally
        {
            sw.Stop();
            if (result is not null)
            {
                if (result.IsValid)
                {
                    EventsMeters.Dispatcher_RequestsProcessed.Add(1);
                }
                else
                {
                    EventsMeters.Dispatcher_RequestsFailed.Add(1);
                }

                EventsMeters.Dispatcher_RequestDurationMs.Record(sw.Elapsed.TotalMilliseconds);
            }
            logger.LogInformation("Command {CommandType} execution completed in {ElapsedMilliseconds}ms", commandTypeName, sw.ElapsedMilliseconds);
        }
    }

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));
        var handlers = serviceProvider.GetServices(handlerType).Cast<object>().ToList();

        using var activity = EventsTracing.ActivitySource.StartActivity("dispatcher.publish", ActivityKind.Producer);
        activity?.SetTag("dispatcher.notification.type", typeof(TNotification).FullName);

        foreach (var handler in handlers)
        {
            using var handlerActivity = EventsTracing.ActivitySource.StartActivity("notification.handler", ActivityKind.Internal);
            handlerActivity?.SetTag("dispatcher.notification.handler", handler.GetType().FullName);

            await (Task)handlerType.GetMethod("Handle")!.Invoke(handler, new object[] { notification!, cancellationToken })!;
        }
    }

    private async Task<Dictionary<string, string[]>> ValidateAsync(ILogger logger, Type commandType, object request, CancellationToken cancellationToken)
    {
        var commandTypeName = commandType.FullName!.Split(".").Last().Replace("+", ".");
        using var validationActivity = EventsTracing.ActivitySource.StartActivity("dispatcher.validation", ActivityKind.Internal);
        validationActivity?.SetTag("dispatcher.request.type", commandTypeName);

        // Prefer IValidator<T> (supports multiple validators)
        var validatorInterface = typeof(AbstractValidator<>).MakeGenericType(commandType);
        var validators = serviceProvider.GetServices(validatorInterface).Cast<IValidator>().ToList();
        validationActivity?.SetTag("validation.validators.count", validators.Count);

        var errorSummary = new Dictionary<string, string[]>();

        if (validators.Count == 0)
        {
            logger.LogDebug("No validators found for {CommandType}", commandTypeName);
            return errorSummary;
        }

        foreach (var validator in validators)
        {
            var sw = Stopwatch.StartNew();
            var validatorName = validator.GetType().FullName!.Split(".").Last().Replace("+", ".");

            using var validatorSpan = EventsTracing.ActivitySource.StartActivity("dispatcher.validation.validator", ActivityKind.Internal);
            validatorSpan?.SetTag("validator.name", validatorName);

            logger.LogDebug("Validating command {CommandType} using {Validator}", commandTypeName, validatorName);

            var context = new ValidationContext<object>(request);
            var result = await validator.ValidateAsync(context, cancellationToken);

            sw.Stop();

            validatorSpan?.SetTag("validation.elapsed_ms", sw.ElapsedMilliseconds);
            validatorSpan?.SetTag("validation.is_valid", result.IsValid);

            if (!result.IsValid)
            {
                foreach (var err in result.Errors)
                {
                    if (err.ErrorMessage == "ignore me!") continue;

                    if (!errorSummary.ContainsKey(err.PropertyName))
                    {
                        errorSummary[err.PropertyName] = new[] { err.ErrorMessage };
                    }
                    else
                    {
                        errorSummary[err.PropertyName] = errorSummary[err.PropertyName].Append(err.ErrorMessage).ToArray();
                    }

                    // Emit a lightweight event per error (keeps spans small but debuggable)
                    validatorSpan?.AddEvent(new ActivityEvent(
                        "validation.error",
                        tags: new ActivityTagsCollection
                        {
                            { "property", err.PropertyName },
                            { "code", err.ErrorCode },
                            { "message", err.ErrorMessage },
                            { "attempted_value", err.AttemptedValue?.ToString() ?? string.Empty }
                        }));
                }
            }

            logger.LogDebug("Validator {Validator} finished in {duration}ms", validatorName, sw.ElapsedMilliseconds);
        }

        validationActivity?.SetTag("validation.error.count", errorSummary.Sum(kv => kv.Value.Length));
        return errorSummary;
    }

    private void PropagateAuthenticatedUser<TResponse>(ref IRequest<TResponse> request)
    {
        if (request is IAuthenticatedRequest authenticatedRequest)
        {
            if (httpContextAccessor?.HttpContext == null)
                return;

            var user = httpContextAccessor.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var claims = user.Claims
                    .Where(x => x.Type == JwtClaimIdentifiers.Roles)
                    .Select(x => x.Value)
                    .ToArray();

                authenticatedRequest.SetIdentity(httpContextAccessor.GetTenant(), httpContextAccessor.GetUsername(), claims);
            }
        }
	}

    private Type? TryResolveHandlerByRequestInterface<TResponse>(Type requestConcreteType, Activity? resolveActivity)
        where TResponse : BaseResponse
    {
        var requestMarkers = GetRequestMarkerInterfaces(requestConcreteType)
            .Distinct()
            .ToArray();

        if (requestMarkers.Length == 0)
        {
            resolveActivity?.SetTag("handler.fallback.marker", "none");
            return null;
        }

        resolveActivity?.SetTag("handler.fallback.marker.count", requestMarkers.Length);
        resolveActivity?.SetTag("handler.fallback.marker.list", string.Join(",", requestMarkers.Select(m => m.FullName)));

        var openHandler = typeof(IRequestHandler<,>);

        // Scan types from loaded assemblies (DI does not expose registrations)
        var candidateContracts = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t is not null)!; }
            })
            .Where(t => t is not null)
            .SelectMany(t => t!.GetInterfaces())
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openHandler)
            .Distinct();

        foreach (var contract in candidateContracts)
        {
            if (contract.GenericTypeArguments.Length != 2)
                continue;

            if (contract.GenericTypeArguments[1] != typeof(TResponse))
                continue;

            var handlerRequestType = contract.GenericTypeArguments[0];

            var handlerMarkers = GetRequestMarkerInterfaces(handlerRequestType)
                .Distinct()
                .ToArray();

            if (handlerMarkers.Length == 0)
                continue;

            var matchedMarker = requestMarkers.FirstOrDefault(rm => handlerMarkers.Contains(rm));
            if (matchedMarker is not null)
            {
                // Ensure it's actually registered
                if (serviceProvider.GetService(contract) is not null)
                {
                    resolveActivity?.AddEvent(new ActivityEvent("handler.fallback.match",
                        tags: new ActivityTagsCollection
                        {
                            { "handler.contract", contract.FullName ?? string.Empty },
                            { "marker", matchedMarker.FullName ?? string.Empty }
                        }));
                    return contract;
                }
            }
        }

        resolveActivity?.SetTag("handler.fallback.match", false);
        return null;
    }

	private Type? TryGetOrResolveHandlerContractType<TResponse>(Type requestConcreteType, Activity? resolveActivity)
		where TResponse : BaseResponse
	{
        return HandlerContractCache.GetOrAdd((requestConcreteType, typeof(TResponse)), key =>
		{
			var direct = typeof(IRequestHandler<,>).MakeGenericType(key.RequestType, key.ResponseType);
			if (serviceProvider.GetService(direct) is not null)
			{
				resolveActivity?.SetTag("handler.match", "direct");
				return direct;
			}

			var fallback = TryResolveHandlerByRequestInterface<TResponse>(key.RequestType, resolveActivity);
			if (fallback is not null)
			{
				resolveActivity?.SetTag("handler.match", "fallback");
			}

			return fallback;
		});
	}

    private static IEnumerable<Type> GetRequestMarkerInterfaces(Type type)
    {
        return type
            .GetInterfaces()
            .Where(i =>
                i.IsInterface &&
                i.Name.EndsWith("Request", StringComparison.Ordinal) &&
                i != typeof(IQuery) &&
                i != typeof(ICommand) &&
                !(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>)) &&
                !(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));
    }
}

public class CommandResponseFactory
{
    public static BaseResponse CreateFailed<T>(T request, ProblemDetails problemDetails) where T : class
    {
        if (request is IQuery)
        {
            return QueryResponse.Failure(problemDetails);
        }
        else if (request is ICommand)
        {
            return CommandResponse.Failure(problemDetails);
        }
        else
        {
            // Check if it's a typed query
            var requestType = request.GetType();
            var queryInterface = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

            if (queryInterface != null)
            {
                var responseType = queryInterface.GetGenericArguments()[0];
                var failureMethod = typeof(QueryResponse<>).MakeGenericType(responseType)
                    .GetMethod("Failure", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(ProblemDetails) }, null);
                return (BaseResponse)failureMethod!.Invoke(null, new object[] { problemDetails })!;
            }

            throw new NotSupportedException($"Request type {request.GetType().FullName} is not supported for command response creation.");
        }
    }
}
