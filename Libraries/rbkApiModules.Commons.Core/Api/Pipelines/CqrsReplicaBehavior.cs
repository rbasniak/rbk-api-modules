using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Core.CQRS;

namespace rbkApiModules.Commons.Core.Pipelines;

public class CqrsReplicaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly IEnumerable<ICqrsReadStore> _contexts;
    private readonly IMapper _mapper;
    private readonly SimpleCqrsBehaviorOptions _storeConfiguration;

    public CqrsReplicaBehavior(ILogger<TRequest> logger, IMapper mapper, 
        IEnumerable<ICqrsReadStore> contexts, SimpleCqrsBehaviorOptions storeConfiguration)
    {
        _logger = logger;
        _mapper = mapper;
        _contexts = contexts;
        _storeConfiguration = storeConfiguration;
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

            var context = GetContext();

            foreach (var entity in entitiesToProcess)
            {
                var mappedEntity = (BaseEntity)_mapper.Map(response.Result, response.Result.GetType(), destinatinoType);

                mappedResults.Add(mappedEntity);

                var existingReplica = await context.FindAsync(mappedEntity.GetType(), mappedEntity.Id);

                var exists = existingReplica != null;

                if (!exists)
                {
                    await context.AddAsync(mappedEntity);
                }
                else
                {
                    await context.UpdateAsync(mappedEntity);
                }
            }

            await context.SaveChangesAsync();

            response.Result = isResponseAnIEnumerable? mappedResults : mappedResults.First();
        }

        return response;
    }

    private ICqrsReadStore GetContext()
    {
        throw new NotImplementedException();
    }
}