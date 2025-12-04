using rbkApiModules.Identity.Core;

namespace Demo1;

public class PasswordPolicyValidator : ICustomPasswordPolicyValidator
{
    private readonly ILocalizationService _localizationService;
    public PasswordPolicyValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<PasswordValidationResult> Validate(string password)
    {
        if (String.IsNullOrEmpty(password)) return await Task.FromResult(PasswordValidationResult.Success()); // To properly test default validators

        var errors = new List<string>();

        if (password.Length < 3)
        {
            errors.Add(_localizationService.LocalizeString(ApplicationMessages.AuthenticationPolicies.Errors.PasswordMustHave3Characters));
        }

        if (errors.Any())
        {
            return await Task.FromResult(PasswordValidationResult.Failure(errors.ToArray()));
        }

        return await Task.FromResult(PasswordValidationResult.Success());
    } 
}