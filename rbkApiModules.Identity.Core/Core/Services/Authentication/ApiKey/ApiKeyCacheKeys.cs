namespace rbkApiModules.Identity.Core;

public static class ApiKeyCacheKeys
{
    public static string AuthenticationEntry(string keyHash)
    {
        return $"rbk:apikey:auth:{keyHash}";
    }

    public static string RateLimitPolicy(string keyHash)
    {
        return $"rbk:apikey:ratelimit:{keyHash}";
    }
}
