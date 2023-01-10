using AutoMapper;
using System.Reflection;

namespace rbkApiModules.Commons.Core;

public static class AutoMapperExtensions
{
    public static void ApplyProfiles(this IMapperConfigurationExpression builder, Assembly[] assemblies)
    {
        var applyConcreteMethod = typeof(IMapperConfigurationExpression).GetMethods()
            .Where(m => m.Name == nameof(IMapperConfigurationExpression.AddProfile)).FirstOrDefault();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(Profile)) && !x.IsGenericType))
            {
                applyConcreteMethod.Invoke(builder, new object[] { Activator.CreateInstance(type) });
            }
        }
    }
}