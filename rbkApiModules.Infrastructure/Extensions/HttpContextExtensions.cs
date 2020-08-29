using Microsoft.AspNetCore.Http;

namespace rbkApiModules.Infrastructure
{
    public static class HttpContextExtensions
    {
        public static string GetUsername(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
        }
    }
}
