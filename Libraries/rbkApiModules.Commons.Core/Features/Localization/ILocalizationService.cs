using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Extensions;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace rbkApiModules.Commons.Core.Localization;

public interface ILocalizationService
{
    string LocalizeString(Enum value);

    string GetLanguageTemplate(string localization = null);
}

public class LocalizationService : ILocalizationService
{
    private static SortedDictionary<string, SortedDictionary<string, string>> _languagesCache = new();
    private static SortedDictionary<string, string> _defaultValues = new();

    private readonly string _systemLanguage = "en-us";
    private readonly string _currentLanguage = "en-us";


    public LocalizationService(IHttpContextAccessor httpContextAccessor, RbkApiCoreOptions coreOptions)
    {
        if (coreOptions != null)
        {
            _systemLanguage = coreOptions._defaultLocalization.ToLower();
        }

        if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
        {
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("localization", out var languageHeaderValues);

            if (languageHeaderValues.Count() > 0)
            {
                _currentLanguage = languageHeaderValues[0].ToLower();
            }
            else
            {
                _currentLanguage = _systemLanguage;
            }
        }
        else
        {
            _currentLanguage = _systemLanguage;
        }

        if (_defaultValues.Count == 0)
        {
            LoadDefaultValues();

            LoadLocalizedValues();
        }
    }

    private void LoadLocalizedValues()
    {
        _languagesCache.Add("en-us", _defaultValues);

        var resources = Assembly.GetAssembly(typeof(ILocalizationService)).GetManifestResourceNames().Where(x => x.EndsWith(".localization")).ToList();

        foreach (var resource in resources)
        {
            var data = new StreamReader(Assembly.GetAssembly(typeof(ILocalizationService)).GetManifestResourceStream(resource)).ReadToEnd();

            var newDictionary = JsonSerializer.Deserialize<SortedDictionary<string, string>>(data);

            var localization = resource.ToLower().Replace(".localization", "").Split('.').Last().Split('_').Last();

            if (!_languagesCache.ContainsKey(localization))
            {
                var tempDictionary = new SortedDictionary<string, string>();

                foreach (var localizedString in _defaultValues)
                {
                    tempDictionary.Add(localizedString.Key, localizedString.Value);
                }

                _languagesCache.Add(localization.ToLower(), tempDictionary);
            }

            var existingDictionary = _languagesCache[localization];

            foreach (var newEntry in newDictionary)
            {
                if (existingDictionary.ContainsKey(newEntry.Key))
                {
                    existingDictionary[newEntry.Key] = newEntry.Value;
                }
                else
                {
                    existingDictionary.Add(newEntry.Key, newEntry.Value);
                }
            }
        }
    }

    private void LoadDefaultValues()
    {
        _defaultValues = new SortedDictionary<string, string>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var localizableEnums = assemblies
            .SelectMany(assembly => assembly.GetTypes().Where(type => typeof(ILocalizedResource).IsAssignableFrom(type) && !type.IsInterface)
                .SelectMany(typeImplementingInterface => typeImplementingInterface.GetNestedTypes().Where(x => x.IsEnum)))
                    .ToList();

        foreach (var localizableEnum in localizableEnums)
        {
            var enumValues = Enum.GetValues(localizableEnum);

            foreach (var enumValue in enumValues)
            {
                _defaultValues.Add(GetLocalizedEnumIdentifier((Enum)enumValue), GetEnumDescription((Enum)enumValue));
            }
        }
    }

    public string LocalizeString(Enum value)
    {
        var dictionary = _defaultValues;

        if (_languagesCache.ContainsKey(_currentLanguage))
        {
            dictionary = _languagesCache[_currentLanguage];
        }

        var key = GetLocalizedEnumIdentifier(value);

        if (dictionary.TryGetValue(key, out var response))
        {
            return response;
        }
        else
        {
            return GetEnumDescription(value);
        }
    }

    public string GetLanguageTemplate(string localization = null)
    {
        var dictionary = _defaultValues;

        if (localization != null && _languagesCache.ContainsKey(localization))
        {
            dictionary = _languagesCache[localization];
        }

        return JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
    }

    private string GetEnumDescription(Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name != null)
        {
            var field = type.GetField(name);
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
        }

        return value.ToString();
    }

    private string GetLocalizedEnumIdentifier(Enum enumValue)
    {
        var localizableEnum = enumValue.GetType();

        var prefix = localizableEnum.FullName.Split('.').Last().Replace("+", "::");

        if (localizableEnum.FullName.StartsWith("rbk"))
        {
            prefix = "rbkApiModules::" + prefix;
        }

        return $"{prefix}::{enumValue.ToString()}";
    }

}
