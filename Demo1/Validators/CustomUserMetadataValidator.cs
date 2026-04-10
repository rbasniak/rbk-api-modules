using rbkApiModules.Identity.Core;

namespace Demo1;

public class CustomUserMetadataValidator : ICustomUserMetadataValidator
{
    private readonly ILocalizationService _localizationService;

    public CustomUserMetadataValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public Task<UserMetadataValidationResult> Validate(Dictionary<string, string> metadata)
    {
        var errors = new Dictionary<string, List<string>>();

        if (metadata.TryGetValue("sector", out var sector))
        {
            if (String.IsNullOrEmpty(sector))
            {
                errors.Add("Sector", [ _localizationService.LocalizeString(MetadataValidationMessages.General.SectorIsRequired) ]);
            }
        }
        else
        {
            errors.Add("Sector", [ _localizationService.LocalizeString(MetadataValidationMessages.General.SectorIsRequired) ]);
        }

        if (metadata.TryGetValue("age", out var ageStr))
        {
            if (Int32.TryParse(ageStr, out var age))
            {
                if (age < 18)
                {
                    errors.Add("Age", [ _localizationService.LocalizeString(MetadataValidationMessages.General.AgeMustBeGreaterThan18) ]);
                }
            }
            else
            {
                errors.Add("Age", [ _localizationService.LocalizeString(MetadataValidationMessages.General.AgeIsNotValid) ]);
            }
        }
        else
        {
            errors.Add("Age", [ _localizationService.LocalizeString(MetadataValidationMessages.General.AgeIsRequired) ]);
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(UserMetadataValidationResult.Failure(errors));
        }
        else
        {
            return Task.FromResult(UserMetadataValidationResult.Success());
        }
    }
}