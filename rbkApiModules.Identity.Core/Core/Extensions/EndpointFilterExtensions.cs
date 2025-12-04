namespace rbkApiModules.Identity.Core;

public static class EndpointFilterExtensions
{
    public static TBuilder RequireAuthorization<TBuilder>(this TBuilder builder, string requiredClaim)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(new RbkAuthorizationFilter(requiredClaim));
    }
}