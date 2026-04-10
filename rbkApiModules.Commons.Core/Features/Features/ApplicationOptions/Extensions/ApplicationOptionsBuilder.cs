using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core.Features.ApplicationOptions;

namespace rbkApiModules.Commons.Core;

public static class ApplicationOptionsBuilder
{
    private const string SectionSeparator = ": ";
    private const string KeySeparator = "::";

    public static IApplicationBuilder UseApplicationOptions(this IApplicationBuilder app)
    {
        // Block startup until options are loaded
        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        using (var scope = scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<DbContext>();
            if (dbContext == null)
            {
                throw new InvalidOperationException("No DbContext registered. Register your application's DbContext and also call RegisterDbContext<TDbContext>() in AddRbkApiCoreSetup so it is available at startup.");
            }

            // Ensure table exists (in testing we often recreate DB)
            // Do not call Migrate here (handled by SetupDatabase). We just ensure the model is known.

            // Load or create global options for every registered options type (no tenant/user)
            var registrations = scope.ServiceProvider.GetServices<IApplicationOptionsRegistration>();

            foreach (var registration in registrations)
            {
                var options = Activator.CreateInstance(registration.OptionsType)!;
                PopulateOptionsFromDatabase(dbContext, options);
            }

            dbContext.SaveChanges();
        }

        return app;
    }

    private static void PopulateOptionsFromDatabase(DbContext dbContext, object optionsRoot)
    {
        // Traverse properties recursively, prefixing with the settings class name
        var rootName = optionsRoot.GetType().Name;
        TraverseAndApply(dbContext, rootName, optionsRoot, parentKey: rootName);
    }

    private static void TraverseAndApply(DbContext context, string rootName, object currentObject, string parentKey)
    {
        if (currentObject == null) return;

        var set = context.Set<ApplicationOption>();

        var type = currentObject.GetType();

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite) continue;

            var propertyType = property.PropertyType;

            var displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name
                              ?? property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
            var defaultValue = defaultValueAttribute?.Value;

            var key = BuildKey(parentKey, property.Name);

            if (IsSimple(propertyType))
            {
                // Ensure exists in DB at global scope only during seeding
                var option = set.AsNoTracking().SingleOrDefault(x => x.Key == key && x.TenantId == null && x.Username == null);
                if (option == null)
                {
                    var stringValue = defaultValue != null ? ConvertToString(defaultValue, propertyType) : string.Empty;
                    set.Add(new ApplicationOption(key, tenantId: null, username: null, stringValue));
                }
                // Do not set values into the instance here; that is done by the manager on per-user/tenant retrieval
            }
            else if (!propertyType.IsArray && !typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType))
            {
                // Complex object: instantiate if null and go deeper
                var nested = property.GetValue(currentObject);
                if (nested == null)
                {
                    nested = Activator.CreateInstance(propertyType);
                    property.SetValue(currentObject, nested);
                }

                TraverseAndApply(context, rootName, nested, BuildSection(parentKey, property.Name));
            }
            else
            {
                // Collections are not supported for now; skip
                continue;
            }
        }
    }

    private static string BuildKey(string parentKey, string name)
    {
        if (string.IsNullOrWhiteSpace(parentKey)) return name;
        // For leafs, join with KeySeparator between last section and leaf
        var lastSectionSplit = parentKey.Split(SectionSeparator);
        if (lastSectionSplit.Length > 1)
        {
            var sections = string.Join(SectionSeparator, lastSectionSplit.Take(lastSectionSplit.Length - 1));
            var last = lastSectionSplit.Last();
            return string.IsNullOrEmpty(sections)
                ? $"{last}{KeySeparator}{name}"
                : $"{sections}{SectionSeparator}{last}{KeySeparator}{name}";
        }
        else
        {
            return $"{parentKey}{KeySeparator}{name}";
        }
    }

    private static string BuildSection(string parentKey, string name)
    {
        if (string.IsNullOrWhiteSpace(parentKey)) return name;
        return $"{parentKey}{SectionSeparator}{name}";
    }

    private static bool IsSimple(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(Guid)
            || type == typeof(TimeSpan);
    }

    private static string ConvertToString(object value, Type targetType)
    {
        if (value == null) return string.Empty;
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (type.IsEnum) return Enum.GetName(type, value) ?? value.ToString();
        if (type == typeof(DateTime)) return ((DateTime)value).ToString("o");
        if (type == typeof(TimeSpan)) return ((TimeSpan)value).ToString();
        if (type == typeof(bool)) return ((bool)value) ? "true" : "false";
        if (type == typeof(decimal)) return ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(double)) return ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(float)) return ((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static object ConvertFromString(string value, Type targetType)
    {
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (type == typeof(string)) return value;
        if (type == typeof(Guid)) return Guid.Parse(value);
        if (type == typeof(DateTime)) return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
        if (type == typeof(TimeSpan)) return TimeSpan.Parse(value);
        if (type.IsEnum) return Enum.Parse(type, value, ignoreCase: true);
        if (type == typeof(bool)) return value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1";
        if (type == typeof(int)) return int.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(long)) return long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(short)) return short.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(byte)) return byte.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(decimal)) return decimal.Parse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(double)) return double.Parse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(float)) return float.Parse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture);

        // Fallback to TypeConverter
        var converter = TypeDescriptor.GetConverter(type);
        if (converter != null && converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        throw new InvalidOperationException($"Cannot convert option value '{value}' to type {targetType.FullName}");
    }
}

