using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.Utilities.EFCore
{
    public static class ModelBuilderExtensions
    {
        private static bool HasJsonAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo != null && propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(JsonFieldAttribute));
        }

        /// <summary>
        /// Add fields marked with <see cref="JsonFieldAttribute"/> to be converted using <see cref="JsonValueConverter{T}"/>.
        /// </summary>
        /// <param name="skipConventionalEntities">
        ///   Skip trying to initialize properties for entity types found by EF conventions.
        ///   EF conventions treats complex fields as possible entity types. This can easily cause issues if we are cross referencing types utilizing
        ///   JsonAttribute while not registering them as actual entities in our db context.
        /// </param>
        /// <remarks>
        /// Adapted from https://github.com/Innofactor/EfCoreJsonValueConverter
        /// </remarks>
        public static void AddJsonFields(this ModelBuilder modelBuilder, bool skipConventionalEntities = true)
        {
            Debug.WriteLine($"\nSetting up JSON column converters");

            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var typeBase = typeof(TypeBase);
                if (skipConventionalEntities)
                {
                    var typeConfigurationSource = typeBase.GetField("_configurationSource", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entityType)?.ToString();
                    if (Enum.TryParse(typeConfigurationSource, out ConfigurationSource typeSource) && typeSource == ConfigurationSource.Convention)
                    {
                        continue;
                    }
                }

                var ignoredMembers = typeBase.GetField("_ignoredMembers", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entityType) as Dictionary<string, ConfigurationSource>;

                bool NotIgnored(PropertyInfo property) =>
                  property != null && !ignoredMembers.ContainsKey(property.Name) && !property.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute));

                foreach (var clrProperty in entityType.ClrType.GetProperties().Where(x => NotIgnored(x) && HasJsonAttribute(x)))
                {
                    var property = modelBuilder.Entity(entityType.ClrType).Property(clrProperty.PropertyType, clrProperty.Name);
                    var modelType = clrProperty.PropertyType;

                    var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });
                    property.Metadata.SetValueConverter(converter);

                    var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                    property.Metadata.SetValueComparer((ValueComparer)Activator.CreateInstance(valueComparer, new object[0]));
                }
            }
        }

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