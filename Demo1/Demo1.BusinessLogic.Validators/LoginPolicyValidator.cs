using rbkApiModules.Identity.Core;

namespace Demo1.BusinessLogic.Validators;

public class LoginPolicyValidator1 : ICustomLoginPolicyValidator
{
    public async Task<CustomValidationResult> Validate(string tenant, string username, string password)
    {
        if (username == "forbidden")
        {
            return await Task.FromResult(CustomValidationResult.Failure("You tried to login with the forbidden username"));
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
            return await Task.FromResult(CustomValidationResult.Failure("You tried to login with the forbidden tenant"));
        }
        else
        {
            return await Task.FromResult(CustomValidationResult.Success());
        }
    }
}