using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.Utilities.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyConfigurations(this ModelBuilder builder, Assembly[] assemblies)
        {
            var applyGenericMethod = typeof(ModelBuilder).GetMethods()
                .Where(m => m.Name == nameof(ModelBuilder.ApplyConfiguration) &&
                        m.GetParameters().First().ParameterType.GetGenericTypeDefinition() ==
                            typeof(IEntityTypeConfiguration<>).GetGenericTypeDefinition()).FirstOrDefault();

            var isFirstGlobally = true;

            foreach (var assembly in assemblies)
            {
                var isFirstOnAssembly = true;

                foreach (var type in assembly.GetTypes().Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (iface.IsConstructedGenericType &&
                            iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                        {
                            var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                            applyConcreteMethod.Invoke(builder, new object[] { Activator.CreateInstance(type) });

                            var info = type.GetTypeInfo();

                            if (isFirstGlobally)
                            {
                                Debug.WriteLine($"\nSetting up database schema");
                            }

                            if (isFirstOnAssembly)
                            {
                                Debug.WriteLine($"  -> {assembly.GetName().Name}");
                            }

                            Debug.WriteLine($"    -> {info.ImplementedInterfaces.First().GetTypeInfo().GenericTypeArguments[0].Name}");

                            isFirstGlobally = false;
                            isFirstOnAssembly = false;

                            break;
                        }
                    }
                }
            }
        }
    }
}