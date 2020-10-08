using System.Reflection;

namespace rbkApiModules.UIAnnotations
{
    public class UIDefinitionOptions
    {
        public UIDefinitionOptions(Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; set; }
    }
}
