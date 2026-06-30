namespace Demo2;

/// <summary>Fixed integration API key (seeded in <see cref="DatabaseSeed"/>). Secret segment is 64 hex chars.</summary>
public static class DemoIntegrationApiKey
{
    public const string Prefix = "rbk_live";

    public const string Secret = "fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210";

    public const string Value = Prefix + "_" + Secret;
}
