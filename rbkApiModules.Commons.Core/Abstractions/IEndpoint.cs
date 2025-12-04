using Microsoft.AspNetCore.Routing;

namespace rbkApiModules.Commons.Core;

// TODO: remove, don't want black magic happening anymore
public interface IEndpoint
{
    static abstract void MapEndpoint(IEndpointRouteBuilder endpoints);
}
