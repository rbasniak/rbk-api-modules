using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections;
using rbkApiModules.Commons.Core.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Commons.Core.Pipelines;

public class CqrsReplicaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly IMapper _mapper;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;

    //private readonly SimpleCqrsBehaviorOptions _storeConfiguration;

    public CqrsReplicaBehavior(ILogger<TRequest> logger, IMapper mapper, IServiceCollection services, IServiceProvider serviceProvider
        // , SimpleCqrsBehaviorOptions storeConfiguration
        )
    {
        _logger = logger;
        _mapper = mapper;
        _services = services;
        _serviceProvider = serviceProvider;
        // _storeConfiguration = storeConfiguration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        var response = await next();

        var requestInterfaces = request.GetType().GetInterfaces();

        var hasReadingModelInterface = requestInterfaces.FirstOrDefault(x => x.FullName.Contains(typeof(IHasReadingModel<object>).Name));

        if (response.IsValid && hasReadingModelInterface != null)
        {
            var requestGenericArguments = hasReadingModelInterface.GenericTypeArguments;

            var destinatinoType = requestGenericArguments.First();

            var entitiesToProcess = new List<BaseEntity>();

            var isResponseAnIEnumerable = false;

            if (typeof(IEnumerable).IsAssignableFrom(response.Result.GetType()) && response.Result.GetType() != typeof(string))
            {
                isResponseAnIEnumerable = true;

                var requestResultsList = (IEnumerable)response.Result;

                foreach (var entity in requestResultsList)
                {
                    entitiesToProcess.Add((BaseEntity)entity);
                }
            }
            else
            {
                entitiesToProcess.Add((BaseEntity)response.Result);
            }

            var mappedResults = new List<object>();

            dynamic context = GetContext(destinatinoType.GetType()); // TODO: think in a better solution

            foreach (var entity in entitiesToProcess)
            {
                var mappedEntity = (BaseEntity)_mapper.Map(response.Result, response.Result.GetType(), destinatinoType);

                mappedResults.Add(mappedEntity);

                var existingReplica = await context.FindAsync(mappedEntity.Id);

                var exists = existingReplica != null;

                if (!exists)
                {
                    await context.AddAsync(mappedEntity);
                }
                else
                {
                    await context.UpdateAsync(mappedEntity.Id, mappedEntity);
                }
            }

            await context.SaveChangesAsync();

            response.Result = isResponseAnIEnumerable? mappedResults : mappedResults.First();
        }

        return response;
    }

    private object GetContext(Type type)
    {
        var registration =_services.FirstOrDefault(x => x.ServiceType == typeof(ICqrsReadStore<>) && x.ServiceType.GenericTypeArguments.First() == type);

        if (registration == null) throw new InvalidOperationException("service not registered");

        return _serviceProvider.GetService(registration.ImplementationType);
    }
}