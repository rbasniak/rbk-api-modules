namespace rbkApiModules.Commons.Core.Helpers;

public static class TestingEnvironmentChecker
{
    private static bool? _isTestingEnvironment = null;

    public static bool IsTestingEnvironment
    {
        get
        {
            if (_isTestingEnvironment == null)
            {
                _isTestingEnvironment = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.StartsWith("Microsoft") && !x.FullName.StartsWith("System")).Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit") || assembly.FullName.ToLowerInvariant().StartsWith("tunit"));
            }

            return _isTestingEnvironment.Value;
        }
    }
}
