using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Identity.Core;

namespace Demo3;

public class LoginPolicyValidator : ICustomLoginPolicyValidator
{
    private readonly ICustomIdentityProviderService _customIdentifyProviderService;

    public LoginPolicyValidator(ICustomIdentityProviderService customIdentifyProviderService)
    {
        _customIdentifyProviderService = customIdentifyProviderService;
    }

    public Task<CustomValidationResult> Validate(string tenant, string username, string password)
    {
        var userinfo = _customIdentifyProviderService.GetUserInfo(username);

        if (userinfo.IsValid)
        {
            return Task.FromResult(CustomValidationResult.Success());
        }
        else
        {
            return Task.FromResult(CustomValidationResult.Failure(AuthenticationMessages.Validations.InvalidCredentials));
        }
    }
}
  