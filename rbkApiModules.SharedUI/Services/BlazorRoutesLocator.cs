using System.Reflection;

namespace rbkApiModules.SharedUI
{
    public class BlazorRoutesLocator
    {
        private Assembly[] _assemblies;

        public BlazorRoutesLocator(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public Assembly[] RoutingAssemblies => _assemblies;
    }
}
