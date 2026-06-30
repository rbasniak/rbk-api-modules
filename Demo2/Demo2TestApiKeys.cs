namespace Demo2;

/// <summary>Fixed API keys for Demo2 tests (seeded in <see cref="DatabaseSeed"/>). Secret segment is 64 hex chars.</summary>
public static class Demo2TestApiKeys
{
    public const string Prefix = "rbk_live";

    public const string GlobalManageAndCrossTenantSecret =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public const string GlobalManageOnlySecret =
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    public const string BuziosManageSecret =
        "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";

    public const string UnBsManageSecret =
        "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";

    public const string GlobalManageAndCrossTenant = Prefix + "_" + GlobalManageAndCrossTenantSecret;

    public const string GlobalManageOnly = Prefix + "_" + GlobalManageOnlySecret;

    public const string BuziosManage = Prefix + "_" + BuziosManageSecret;

    public const string UnBsManage = Prefix + "_" + UnBsManageSecret;
}
