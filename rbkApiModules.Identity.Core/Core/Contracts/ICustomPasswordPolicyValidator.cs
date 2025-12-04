namespace rbkApiModules.Identity.Core;

public interface ICustomPasswordPolicyValidator
{
    Task<PasswordValidationResult> Validate(string password);
}

public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];

    public static PasswordValidationResult Success() => new PasswordValidationResult { IsValid = true };
    public static PasswordValidationResult Failure(params string[] errors) => new PasswordValidationResult { IsValid = false, Errors = errors.ToList() };
}