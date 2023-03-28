using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Extensions;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Commons.Core.Localization;

public interface ILocalizationService
{
    string GetValue(Enum value);

    string GetLanguageTemplate();
}

public class LocalizationService : ILocalizationService
{
    private static Dictionary<string, Dictionary<string, string>> _languagesCache = new();
    private static Dictionary<string, string> _defaultValues = new();

    private readonly string _systemLanguage = "en-us";
    private readonly string _currentLanguage = "en-us";


    public LocalizationService(IHttpContextAccessor httpContextAccessor, RbkApiCoreOptions coreOptions)
    {
        if (coreOptions != null)
        {
            _systemLanguage = coreOptions._defaultLocalization;
        }

        if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
        {
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("localization", out var languageHeaderValues);

            if (languageHeaderValues.Count() > 0)
            {
                _currentLanguage = languageHeaderValues[0];
            }
        }
        else
        {
            _currentLanguage = _systemLanguage;
        }

        if (_languagesCache.ContainsKey(_currentLanguage))
        {

        }

        if (_defaultValues.Count == 0)
        {
            _defaultValues = EnumIdentifierFinder.FindEnumIdsWithDescriptions();
        }
    } 

    public string GetValue(Enum value)
    {


        return value.GetAttributeOfType<DescriptionAttribute>().Description;
    }

    public string GetLanguageTemplate()
    {
        var builder = new StringBuilder();

        foreach (var kvp in _defaultValues)
        {
            builder.AppendLine($"{kvp.Key}={kvp.Value}");
        }

        return builder.ToString();
    }
}

public static class EnumExtensions
{
    public static string GetLocalizationId(this Enum value)
    {
        var enumType = value.GetType();
        string enumTypeName = enumType.Name;
        string valueName = value.ToString();
        string owningClasses = "";

        Type declaringType = enumType.DeclaringType;

        while (declaringType != null)
        {
            owningClasses = declaringType.Name + "::" + owningClasses;
            declaringType = declaringType.DeclaringType;
        }

        return owningClasses + enumTypeName + "::" + valueName;
    }
}

public static class EnumIdentifierFinder
{
    public static Dictionary<string, string> FindEnumIdsWithDescriptions()
    {
        var enumIdsWithDescriptions = new Dictionary<string, string>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var types = assembly.GetTypes()
                    .Where(x => typeof(ILocalizedResource).IsAssignableFrom(x) && !x.IsInterface);

            foreach (var type in types)
            {
                if (type.IsEnum)
                {
                    foreach (Enum enumValue in Enum.GetValues(type))
                    {
                        string enumId = GetEnumIdentifier(enumValue);
                        string enumDescription = GetEnumValueDescription(enumValue);
                        enumIdsWithDescriptions[enumId] = enumDescription;
                    }
                }
            }
        }

        return enumIdsWithDescriptions;
    }

    private static string GetEnumValueDescription(Enum enumValue)
    {
        FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            DescriptionAttribute descriptionAttribute =
                (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));

            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }
        }

        return enumValue.ToString();
    }

    private static string GetEnumIdentifier(Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        string enumTypeName = enumType.Name;
        string valueName = enumValue.ToString();
        string owningClasses = "";

        Type declaringType = enumType.DeclaringType;
        while (declaringType != null)
        {
            owningClasses = declaringType.Name + "::" + owningClasses;
            declaringType = declaringType.DeclaringType;
        }

        return owningClasses + enumTypeName + "::" + valueName;
    }
}
