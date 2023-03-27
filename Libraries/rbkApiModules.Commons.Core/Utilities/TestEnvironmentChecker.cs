using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Core.Utilities;

public static class TestEnvironmentChecker
{
    public static bool IsTestEnvironment => AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit"));
}
