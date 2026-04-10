using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core.Features.ApplicationOptions;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace rbkApiModules.Commons.Core;

internal sealed class ApplicationOptionsManager : IApplicationOptionsManager
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string CacheKeyPrefix = "AppOptions:";

    public ApplicationOptionsManager(IServiceScopeFactory scopeFactory, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public TOptions GetOptions<TOptions>(string? tenantId = null, string? username = null)
        where TOptions : class, IApplicationOptions, new()
    {
        // If inside HTTP request, derive from HttpContext when not explicitly provided
        string? ctxTenant = null;
        string? ctxUsername = null;

        if (_httpContextAccessor?.HttpContext != null)
        {
            try { ctxTenant = _httpContextAccessor.GetTenant(); } catch { ctxTenant = null; }
            try { ctxUsername = _httpContextAccessor.GetUsername(); } catch { ctxUsername = null; }
        }

        var normalizedUsername = string.IsNullOrWhiteSpace(username) ? null : username.ToLower();

        if (!string.IsNullOrWhiteSpace(ctxTenant) || !string.IsNullOrWhiteSpace(ctxUsername))
        {
            // Validate provided vs context to prevent cross-tenant/user access
            if (!string.IsNullOrWhiteSpace(tenantId) && !string.Equals(ctxTenant, tenantId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Tenant from HttpContext does not match the provided tenant parameter.");
            }

            if (!string.IsNullOrWhiteSpace(normalizedUsername) && !string.Equals(ctxUsername, normalizedUsername, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Username from HttpContext does not match the provided username parameter.");
            }

            // Prefer context-derived when available
            tenantId = ctxTenant ?? tenantId;
            normalizedUsername = ctxUsername ?? normalizedUsername;
        }

        var cacheKey = BuildCacheKey(typeof(TOptions), tenantId, normalizedUsername);

        if (_cache.TryGetValue(cacheKey, out TOptions cached))
        {
            return cached;
        }

        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        var options = new TOptions();

        PopulateFromDatabase(dbContext, options, tenantId, normalizedUsername);

        // Cache with sensible eviction; tenant/user overrides can change, allow short sliding expiration
        _cache.Set(cacheKey, options, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });

        return options;
    }

    private static string BuildCacheKey(Type type, string? tenantId, string? username)
    {
        return $"{CacheKeyPrefix}{type.FullName}:{tenantId ?? "_"}:{username ?? "_"}";
    }

    private static void PopulateFromDatabase(DbContext dbContext, object optionsRoot, string? tenantId, string? username)
    {
        var set = dbContext.Set<ApplicationOption>();

        TraverseAndApply(set, optionsRoot.GetType().Name, optionsRoot, parentKey: optionsRoot.GetType().Name, tenantId, username);
    }

    private static void TraverseAndApply(DbSet<ApplicationOption> set, string rootName, object currentObject, string parentKey, string? tenantId, string? username)
    {
        if (currentObject == null) return;

        var type = currentObject.GetType();

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite) continue;

            var propertyType = property.PropertyType;

            var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
            var defaultValue = defaultValueAttribute?.Value;

            var key = BuildKey(parentKey, property.Name);

            if (IsSimple(propertyType))
            {
                // Resolve value priority: User -> Tenant -> Global
                var userValue = set.AsNoTracking().SingleOrDefault(x => x.Key == key && x.Username == username && x.TenantId == tenantId)?.Value;
                var tenantValue = set.AsNoTracking().SingleOrDefault(x => x.Key == key && x.Username == null && x.TenantId == tenantId)?.Value;
                var globalValue = set.AsNoTracking().SingleOrDefault(x => x.Key == key && x.Username == null && x.TenantId == null)?.Value;

                string? selected = userValue ?? tenantValue ?? globalValue;

                if (!string.IsNullOrWhiteSpace(selected))
                {
                    var typedValue = ConvertFromString(selected, propertyType);
                    property.SetValue(currentObject, typedValue);
                }
                else if (defaultValue != null)
                {
                    property.SetValue(currentObject, defaultValue);
                }
            }
            else if (!propertyType.IsArray && !typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType))
            {
                var nested = property.GetValue(currentObject);
                if (nested == null)
                {
                    nested = Activator.CreateInstance(propertyType);
                    property.SetValue(currentObject, nested);
                }

                TraverseAndApply(set, rootName, nested, BuildSection(parentKey, property.Name), tenantId, username);
            }
        }
    }

    private const string SectionSeparator = ": ";
    private const string KeySeparator = "::";

    private static string BuildKey(string parentKey, string name)
    {
        if (string.IsNullOrWhiteSpace(parentKey)) return name;
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

        var converter = TypeDescriptor.GetConverter(type);
        if (converter != null && converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        throw new InvalidOperationException($"Cannot convert option value '{value}' to type {targetType.FullName}");
    }
}

