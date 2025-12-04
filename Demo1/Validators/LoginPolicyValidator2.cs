using rbkApiModules.Identity.Core;

namespace Demo1;

public class LoginPolicyValidator2 : ICustomLoginPolicyValidator
{
    private readonly ILocalizationService _localizationService;
    public LoginPolicyValidator2(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<LoginValidationResult> Validate(string tenant, string username, string password)
    {
        if (tenant == "FORBIDDEN")
        {
            var message = _localizationService.LocalizeString(ApplicationMessages.AuthenticationPolicies.Errors.TriedToLoginWithForbiddenTenant);
            return await Task.FromResult(LoginValidationResult.Failure(message));
        }
        else
        {
            return await Task.FromResult(LoginValidationResult.Success());
        }
    }
}
