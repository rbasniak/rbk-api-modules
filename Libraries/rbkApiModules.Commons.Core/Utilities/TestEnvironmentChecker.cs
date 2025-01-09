namespace rbkApiModules.Commons.Core.Utilities;

public static class TestingEnvironmentChecker
{
    public static bool IsTestingEnvironment => AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.StartsWith("Microsoft") && !x.FullName.StartsWith("System")).Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit") || assembly.FullName.ToLowerInvariant().StartsWith("tunit"));
}
