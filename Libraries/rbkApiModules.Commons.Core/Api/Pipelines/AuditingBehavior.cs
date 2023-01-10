using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core.Auditing;
using System.Text.Json;

namespace rbkApiModules.Commons.Core.Pipelines;

/// <summary>
/// SCENARIO:
/// For auditing and tracing purposes, each command that changed the database can be saved into a table
/// It can be automatically done in the MediatR pipelines when a request succeeds
/// 
/// NOTES:
/// - Doing this for all commands might not be desirable depending on the application requirements. 
/// But it's easy controllable by creating an interface and implementing it into the desirable commands.
/// - Entries are grouped by individual entities
/// 
/// </summary>
public class AuditingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TRequest> _logger;
    private readonly ITraceLogStore _traceLogStore;

    public AuditingBehavior(ILogger<TRequest> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        var response = await next();

        if (response.IsValid && typeof(IAuditableResponse).IsAssignableFrom(response.GetType()))
        {
            var traceData = response as IAuditableResponse;

            var commandId = Guid.NewGuid();

            var entities = new List<TraceLog>();

            foreach (var aggregateGroup in traceData.AffectedEntities.GroupBy(x => x.AggregateId))
            {
                foreach (var EntityId in aggregateGroup)
                {
                    entities.Add(new TraceLog(commandId, request.GetType().FullName, SanitizedJson(request),
                       _httpContextAccessor.HttpContext.User.Identity.Name, aggregateGroup.Key, EntityId.EntityId));
                }
            }

            await _traceLogStore.Add(entities.ToArray());
        }

        return response;
    }

    private string SanitizedJson(object input)
    {
        var forbiddenKeywords = new[] { "content", "contents", "password", "zipBytes" };
        var json = JsonSerializer.Serialize(input);
        // TODO: implement through attribute
        // TODO: implement without Newtonsoft.Json

        return json;
    }
}