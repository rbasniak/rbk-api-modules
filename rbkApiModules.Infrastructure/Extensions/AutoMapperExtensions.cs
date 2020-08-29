using AutoMapper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.Infrastructure
{
    public static class AutoMapperExtensions
    {
        public static void ApplyProfiles(this IMapperConfigurationExpression builder, Assembly[] assemblies)
        {
            var applyConcreteMethod = typeof(IMapperConfigurationExpression).GetMethods()
                .Where(m => m.Name == nameof(IMapperConfigurationExpression.AddProfile)).FirstOrDefault();

            var isFirstGlobally = true;

            foreach (var assembly in assemblies)
            {
                var isFirstOnAssembly = true;

                foreach (var type in assembly.GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(Profile))))
                {
                    if (isFirstGlobally)
                    {
                        Debug.WriteLine($"\nApplying domain to DTO mappings");
                    }

                    if (isFirstOnAssembly)
                    {
                        Debug.WriteLine($"  -> {assembly.GetName().Name}");
                    }

                    isFirstGlobally = false;
                    isFirstOnAssembly = false;

                    applyConcreteMethod.Invoke(builder, new object[] { Activator.CreateInstance(type) });

                    var info = type.GetTypeInfo();

                    Debug.WriteLine($"    -> {info.Name.Replace("Mappings", "")}");
                }
            }
        }
    }
}
