namespace rbkApiModules.Identity.Core;

public interface ICustomPasswordPolicyValidator
{
    Task<CustomValidationResult> Validate(string password);
}