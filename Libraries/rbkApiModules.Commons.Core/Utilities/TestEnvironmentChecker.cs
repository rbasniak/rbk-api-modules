namespace rbkApiModules.Commons.Core.Utilities;

public static class TestingEnvironmentChecker
{
    public static bool IsTestingEnvironment => AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit"));
}
