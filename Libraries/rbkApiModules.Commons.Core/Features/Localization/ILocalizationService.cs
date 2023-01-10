namespace rbkApiModules.Commons.Core.Localization;

public interface ILocalizationService
{
    string GetValue(string value);
}

public class LocalizationService : ILocalizationService
{
    public string GetValue(string value)
    {
        return value;
    }
}
