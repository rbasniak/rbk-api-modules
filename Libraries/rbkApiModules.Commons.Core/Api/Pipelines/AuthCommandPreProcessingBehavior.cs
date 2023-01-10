using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core.Pipelines;

public class AuthCommandPreProcessingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthCommandPreProcessingBehavior(ILogger<TRequest> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        if (request is IAuthenticatedCommand authenticatedRequest)
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var claims = user.Claims
                    .Where(x => x.Type == JwtClaimIdentifiers.Roles)
                    .Select(x => x.Value)
                    .ToArray();

                authenticatedRequest.SetIdentity(_httpContextAccessor.GetTenant(), _httpContextAccessor.GetUsername(), claims);
            }
        }

        return await next();
    }
}