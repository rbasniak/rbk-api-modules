namespace rbkApiModules.Commons.Core.Utilities;

public static class TestingEnvironmentChecker
{
    public static bool IsTestingEnvironment => AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.Contains("Microsoft.Data.SqlClient")).Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit"));
}
