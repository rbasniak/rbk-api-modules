namespace Demo2;

/// <summary>Fixed API keys for Demo2 tests (seeded in <see cref="DatabaseSeed"/>). Secret segment is 64 hex chars.</summary>
public static class Demo2TestApiKeys
{
    public const string GlobalManageAndCrossTenant =
        "rbk_live_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public const string GlobalManageOnly =
        "rbk_live_bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    public const string BuziosManage =
        "rbk_live_cccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";

    public const string UnBsManage =
        "rbk_live_dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";
}
