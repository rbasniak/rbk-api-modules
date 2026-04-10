namespace rbkApiModules.Identity.Core;

public interface ICustomLoginPolicyValidator
{
    Task<LoginValidationResult> Validate(string tenant, string username, string password);
}

public class LoginValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];

    public static LoginValidationResult Success() => new LoginValidationResult { IsValid = true };
    public static LoginValidationResult Failure(params string[] errors) => new LoginValidationResult { IsValid = false, Errors = errors.ToList() };
}

