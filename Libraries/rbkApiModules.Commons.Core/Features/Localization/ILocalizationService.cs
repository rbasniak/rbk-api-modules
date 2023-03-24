using Microsoft.OpenApi.Extensions;
using System.ComponentModel;

namespace rbkApiModules.Commons.Core.Localization;

public interface ILocalizationService
{
    string GetValue(Enum value); 
}

public class LocalizationService : ILocalizationService
{
    public string GetValue(Enum value)
    {
        return value.GetAttributeOfType<DescriptionAttribute>().Description;
    } 
}
