using System.Reflection;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public static class ReflectionExtensions
{
    public static IgnoreMode GetCodeGenerationIgnoreMode(this Type type) 
    {
        var attribute = type.GetAttribute<IgnoreOnCodeGenerationAttribute>();

        if (attribute != null)
        {
            return attribute.Type;
        }
        else
        {
            return IgnoreMode.None;
        }
    }

    public static StoreBehavior GetNgxsStoreBehavior(this Type type)
    {
        var attribute = type.GetAttribute<NgxsStoreBehaviorAttribute>();

        if (attribute != null)
        {
            return attribute.Behavior;
        }
        else
        {
            return StoreBehavior.General;
        }
    }

    public static IgnoreMode GetCodeGenerationIgnoreMode(this MethodInfo method)
    {
        var attribute = method.GetAttribute<IgnoreOnCodeGenerationAttribute>();

        if (attribute != null)
        {
            return attribute.Type;
        }
        else
        {
            return IgnoreMode.None;
        }
    }

    public static bool HasDateProperty(this Type type) 
    {
        var realType = type;

        if (type.IsList())
        {
            realType = type.GetInterfaces().Single(x => x.Name == typeof(IEnumerable<>).Name).GenericTypeArguments.First();
        }

        if (!realType.IsAssignableTo(typeof(BaseDataTransferObject)))
        {
            if (!realType.Assembly.FullName.Contains("rbpApiModules") && !realType.Assembly.FullName.Contains("DataTransfer") && !realType.FullName.Contains("DataTransfer")) return false;
        }

        return realType.GetProperties().Where(x => x.PropertyType != type).Any(x => x.PropertyType == typeof(DateTime) || x.PropertyType.HasDateProperty());
    }
}
