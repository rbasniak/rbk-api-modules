using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Identity.Core;
using System.ComponentModel;

namespace Demo1.BusinessLogic.Validators;

public class LoginPolicyValidator1 : ICustomLoginPolicyValidator
{
    public async Task<CustomValidationResult> Validate(string tenant, string username, string password)
    {
        if (username == "forbidden")
        {
            return await Task.FromResult(CustomValidationResult.Failure(ApplicationMessages.AuthenticationPolicies.Errors.TriedToLoginWithForbiddenUsername));
        }
        else
        {
            return await Task.FromResult(CustomValidationResult.Success());
        }
    }
}

public class LoginPolicyValidator2 : ICustomLoginPolicyValidator
{
    public async Task<CustomValidationResult> Validate(string tenant, string username, string password)
    {
        if (tenant == "FORBIDDEN")
        {
            return await Task.FromResult(CustomValidationResult.Failure(ApplicationMessages.AuthenticationPolicies.Errors.TriedToLoginWithForbiddenTenant));
        }
        else
        {
            return await Task.FromResult(CustomValidationResult.Success());
        }
    }
}

public class ApplicationMessages: ILocalizedResource
{
    public class AuthenticationPolicies
    {
        public enum Errors
        {
            [Description("You tried to login with the forbidden tenant")] TriedToLoginWithForbiddenTenant,
            [Description("You tried to login with the forbidden username")] TriedToLoginWithForbiddenUsername,
            [Description("The password must have at least 3 characteres")] PasswordMustHave3Characters,
        }
    }
}