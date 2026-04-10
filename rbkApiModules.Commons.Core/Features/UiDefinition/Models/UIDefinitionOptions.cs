using System.Reflection;

namespace rbkApiModules.Commons.Core.UiDefinitions;

public class UIDefinitionOptions
{
    public UIDefinitionOptions(Assembly[] assemblies)
    {
        Assemblies = assemblies;
    }

    public Assembly[] Assemblies { get; set; }
}
