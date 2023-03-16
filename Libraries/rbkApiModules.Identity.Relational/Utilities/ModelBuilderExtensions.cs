using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity;

public static class ModelBuilderExtensions
{
    public static void SetupTenants(this ModelBuilder modelBuilder)
    {
        if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            var typeBase = typeof(TypeBase);

            var ignoredMembers = typeBase.GetField("_ignoredMembers", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entityType) as Dictionary<string, ConfigurationSource>;

            bool NotIgnored(PropertyInfo property) =>
                property != null && !ignoredMembers.ContainsKey(property.Name) && !property.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute));

            var allProperties = entityType.ClrType.GetProperties().ToList();

            var properties = allProperties.Where(x => NotIgnored(x) && x.PropertyType == typeof(string) && x.Name == nameof(TenantEntity.TenantId));

            foreach (var tenantProperty in properties)
            {
                Debug.WriteLine(entityType.Name + "::" + tenantProperty.Name + " >> " + entityType.ClrType.Name);

                modelBuilder
                    .Entity(entityType.ClrType, x =>
                    {
                        x.Property<string>(nameof(TenantEntity.TenantId));
                        x.HasOne(typeof(Tenant)).WithMany().HasForeignKey(nameof(TenantEntity.TenantId));

                    });
            }
        }
    }
}