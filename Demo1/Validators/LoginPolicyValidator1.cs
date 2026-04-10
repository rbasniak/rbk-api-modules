using rbkApiModules.Identity.Core;

namespace Demo1;

public class LoginPolicyValidator1 : ICustomLoginPolicyValidator
{
    private readonly ILocalizationService _localizationService;
    public LoginPolicyValidator1(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<LoginValidationResult> Validate(string tenant, string username, string password)
    {
        if (username == "forbidden")
        {
            var message = _localizationService.LocalizeString(ApplicationMessages.AuthenticationPolicies.Errors.TriedToLoginWithForbiddenUsername);
            return await Task.FromResult(LoginValidationResult.Failure(message));
        }
        else
        {
            return await Task.FromResult(LoginValidationResult.Success());
        }
    }
}

