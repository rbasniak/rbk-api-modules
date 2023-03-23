namespace rbkApiModules.Identity.Core;

public interface ICustomUserMetadataValidator
{
    Task<CustomValidationResult> Validate(Dictionary<string, string> userMetadata);
}