using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace rbkApiModules.Commons.Core.Localization;

public interface ILocalizationService
{
    string LocalizeString(Enum value);

    string GetLanguageTemplate(string localization = null);
}

public class LocalizationCache
{
    public SortedDictionary<string, SortedDictionary<string, string>> LanguagesCache = new();
    public SortedDictionary<string, string> DefaultValues = new();

    public LocalizationCache()
    {
        Log.Logger.Information("Localization cache is not initialized");

        LoadDefaultValues();

        LoadLocalizedValues();
    }

    private void LoadLocalizedValues()
    {
        Log.Logger.Information("Looking for localization files in resources");

        LanguagesCache.Add("en-us", DefaultValues);

        var resources = Assembly.GetAssembly(typeof(ILocalizationService)).GetManifestResourceNames().Where(x => x.EndsWith(".localization")).ToList();

        foreach (var resource in resources)
        {
            Log.Logger.Information("Processing localization resource file {resource}", resource);

            var data = new StreamReader(Assembly.GetAssembly(typeof(ILocalizationService)).GetManifestResourceStream(resource)).ReadToEnd();

            var newDictionary = JsonSerializer.Deserialize<SortedDictionary<string, string>>(data);

            var localization = resource.ToLower().Replace(".localization", "").Split('.').Last().Split('_').Last();

            if (!LanguagesCache.ContainsKey(localization))
            {
                Log.Logger.Information("Dictionary for language nof found, initializing with default en-us values");

                var tempDictionary = new SortedDictionary<string, string>();

                foreach (var localizedString in DefaultValues)
                {
                    tempDictionary.Add(localizedString.Key, localizedString.Value);
                }

                LanguagesCache.Add(localization.ToLower(), tempDictionary);

                Log.Logger.Information("{language} initialized successfully", localization);
            }

            var existingDictionary = LanguagesCache[localization];


            foreach (var newEntry in newDictionary)
            {
                if (existingDictionary.ContainsKey(newEntry.Key))
                {
                    Log.Logger.Information("Replacing value for key {key}", newEntry.Key);

                    existingDictionary[newEntry.Key] = newEntry.Value;
                }
                else
                {
                    Log.Logger.Information("Adding key {key}", newEntry.Key);

                    existingDictionary.Add(newEntry.Key, newEntry.Value);
                }
            }
        }

        Log.Logger.Information("Localization files loaded successfully");
    }

    private void LoadDefaultValues()
    {
        Log.Logger.Information("Initializing default localized values for en-us");

        DefaultValues = new SortedDictionary<string, string>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var localizableEnums = assemblies
            .SelectMany(assembly => assembly.GetTypes().Where(type => typeof(ILocalizedResource).IsAssignableFrom(type) && !type.IsInterface)
                .SelectMany(typeImplementingInterface => typeImplementingInterface.GetNestedTypes().Where(x => x.IsEnum)))
                    .ToList();

        foreach (var localizableEnum in localizableEnums)
        {
            Log.Logger.Information("Processing enum {enum}", localizableEnum.FullName.Split('.').Last());

            var enumValues = Enum.GetValues(localizableEnum);

            foreach (var enumValue in enumValues)
            {
                var key = ((Enum)enumValue).GetLocalizedEnumIdentifier();
                var value = ((Enum)enumValue).GetEnumDescription();

                Log.Logger.Information("Adding key {key}", key);

                DefaultValues.Add(key, value);
            }
        }
    }
}

public class LocalizationService : ILocalizationService
{
    private readonly string _systemLanguage = "en-us";
    private readonly string _currentLanguage = "en-us";

    private readonly LocalizationCache _localizationCache;

    public LocalizationService(IHttpContextAccessor httpContextAccessor, RbkApiCoreOptions coreOptions, LocalizationCache localizationCache)
    {
        Log.Logger.Information("Instantiating service: {service}", nameof(LocalizationService));

        _localizationCache = localizationCache;

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

        Log.Logger.Information("Using {language} for _systemLanguage", _systemLanguage);
        Log.Logger.Information("Using {language} for _currentLanguage", _currentLanguage);
    }

    public string LocalizeString(Enum value)
    {
        var dictionary = _localizationCache.DefaultValues;

        if (_localizationCache.LanguagesCache.ContainsKey(_currentLanguage))
        {
            dictionary = _localizationCache.LanguagesCache[_currentLanguage];
        }

        var key = value.GetLocalizedEnumIdentifier();

        if (dictionary.TryGetValue(key, out var response))
        {
            return response;
        }
        else
        {
            return value.GetEnumDescription();
        }
    }

    public string GetLanguageTemplate(string localization = null)
    {
        var dictionary = _localizationCache.DefaultValues;

        if (localization != null && _localizationCache.LanguagesCache.ContainsKey(localization))
        {
            dictionary = _localizationCache.LanguagesCache[localization];
        }

        return JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
    }
}

file static class Extensions
{
    public static string GetLocalizedEnumIdentifier(this Enum enumValue)
    {
        var localizableEnum = enumValue.GetType();

        var prefix = localizableEnum.FullName.Split('.').Last().Replace("+", "::");

        if (localizableEnum.FullName.StartsWith("rbk"))
        {
            prefix = "rbkApiModules::" + prefix;
        }

        return $"{prefix}::{enumValue.ToString()}";
    }

    public static string GetEnumDescription(this Enum value)
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
}
