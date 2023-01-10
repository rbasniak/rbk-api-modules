namespace rbkApiModules.Identity.Core;

public interface ICustomLoginPolicyValidator
{
    Task<CustomValidationResult> Validate(string tenant, string username, string password);
}