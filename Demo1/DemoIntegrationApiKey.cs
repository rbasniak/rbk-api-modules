namespace Demo1;

/// <summary>Fixed integration API key (seeded in <see cref="DatabaseSeed"/>). Secret segment is 64 hex chars.</summary>
public static class DemoIntegrationApiKey
{
    public const string Prefix = "rbk_live";

    public const string Secret = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    public const string Value = Prefix + "_" + Secret;
}
