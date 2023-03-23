namespace rbkApiModules.Identity.Core;

public interface ICustomLoginPolicyValidator
{
    Task<CustomValidationResult> Validate(string tenant, string username, string password);
}

public interface ICustomUserMetadataValidator
{
    Task<CustomValidationResult> Validate(Dictionary<string, string> userMetadata);
}