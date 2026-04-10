namespace rbkApiModules.Identity.Core;

public interface ICustomUserMetadataValidator
{
    Task<UserMetadataValidationResult> Validate(Dictionary<string, string> userMetadata);
}

public class UserMetadataValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = new();

    public static UserMetadataValidationResult Success() => new UserMetadataValidationResult { IsValid = true };
    public static UserMetadataValidationResult Failure(Dictionary<string, List<string>> errors) => new UserMetadataValidationResult { IsValid = false, Errors = errors };
}