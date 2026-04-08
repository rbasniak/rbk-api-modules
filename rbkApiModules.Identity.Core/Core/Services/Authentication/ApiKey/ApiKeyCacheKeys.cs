namespace rbkApiModules.Identity.Core;

public static class ApiKeyCacheKeys
{
    public static string AuthenticationEntry(string keyHash)
    {
        return $"rbk:apikey:auth:{keyHash}";
    }
}
