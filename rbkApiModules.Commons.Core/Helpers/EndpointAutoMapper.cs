using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace rbkApiModules.Commons.Core;

public static class EndpointAutoMapper
{
    public static void MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => !x.GetName().Name.Contains("Microsoft"))
            .Where(x => x.GetName().Name.Contains("PaintingProjectsManagement"))
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod(nameof(IEndpoint.MapEndpoint), BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, [ app ]);
        }
    }
}
