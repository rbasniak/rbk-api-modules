﻿using rbkApiModules.Identity.Core;

namespace Demo1.BusinessLogic.Validators;

public class PasswordPolicyValidator : ICustomPasswordPolicyValidator
{
    public async Task<CustomValidationResult> Validate(string password)
    {
        if (password.Length < 3)
        {
            return await Task.FromResult(CustomValidationResult.Failure(ApplicationMessages.AuthenticationPolicies.Errors.PasswordMustHave3Characters));
        }

        return await Task.FromResult(CustomValidationResult.Success());
    }
}