using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Identity.Core;
using System.ComponentModel;

namespace Demo3;

public class CustomUserMetadataValidator : ICustomUserMetadataValidator
{
    public Task<CustomValidationResult> Validate(Dictionary<string, string> metadata)
    {
        var errors = new List<Enum>();

        if (metadata.TryGetValue("sector", out var sector))
        {
            if (String.IsNullOrEmpty(sector))
            {
                errors.Add(MetadataValidationMessages.General.SectorIsRequired);
            }
        }
        else
        {
            errors.Add(MetadataValidationMessages.General.SectorIsRequired);
        }

        if (metadata.TryGetValue("age", out var ageStr))
        {
            if (Int32.TryParse(ageStr, out var age))
            {
                if (age < 18)
                {
                    errors.Add(MetadataValidationMessages.General.AgeMustBeGreaterThan18);
                }
            }
            else
            {
                errors.Add(MetadataValidationMessages.General.AgeIsNotValid);
            }
        }
        else
        {
            errors.Add(MetadataValidationMessages.General.AgeIsRequired);
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(CustomValidationResult.Failure(errors.ToArray()));
        }
        else
        {
            return Task.FromResult(CustomValidationResult.Success());
        }
    }
}

public class MetadataValidationMessages: ILocalizedResource
{
    public enum General
    {
        [Description("Sector is required")] SectorIsRequired,
        [Description("Age must be greater than 18")] AgeMustBeGreaterThan18,
        [Description("Age is required")] AgeIsRequired,
        [Description("Age is not valid")] AgeIsNotValid,
    }
}