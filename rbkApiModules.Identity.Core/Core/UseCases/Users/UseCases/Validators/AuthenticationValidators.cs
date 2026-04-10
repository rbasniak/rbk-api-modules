using rbkApiModules.Identity.Core;

internal static class AuthenticationValidators
{
    public static IRuleBuilderOptions<T, AuthenticatedUser> HasCorrectRoleManagementAccessRights<T>(this IRuleBuilder<T, AuthenticatedUser> rule, ILocalizationService localization)
    {
        return rule
            .Must(identity =>
            {
                if (String.IsNullOrEmpty(identity.Tenant))
                {
                    return identity.HasClaim(AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES);
                }
                else
                {
                    return identity.HasClaim(AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES);
                }
            })
            .WithMessage(command => localization.LocalizeString(AuthenticationMessages.Validations.UnauthorizedAccess));
    }
 
    public static IRuleBuilderOptions<T, string> PasswordPoliciesAreValid<T>(this IRuleBuilder<T, string> rule, IEnumerable<ICustomPasswordPolicyValidator> validators, ILocalizationService localization)
    {
        return rule
            .MustAsync(async (command, password, validationContext, cancellationToken) =>
            {
                bool hasError = false;

                foreach (var validator in validators)
                {
                    var result = await validator.Validate(password);

                    hasError = hasError || result.Errors.Any();

                    foreach (var message in result.Errors)
                    {
                        validationContext.AddFailure("Password", message);
                    }
                }

                return !hasError;
            })
            .WithMessage("ignore me!"); // DO NOT CHANGE THIS MESSAGE! It's used in the pipeline to idenfity these errors
    }

    public static IRuleBuilderOptions<T, ILoginData> LoginPoliciesAreValid<T>(this IRuleBuilder<T, ILoginData> rule, IEnumerable<ICustomLoginPolicyValidator> validators, ILocalizationService localization) where T : ILoginData
    {
        return rule
            .MustAsync(async (command, property, validationContext, cancellationToken) =>
            {
                bool hasError = false;

                foreach (var validator in validators)
                {
                    var result = await validator.Validate(command.Tenant, command.Username, command.Password);

                    hasError = hasError || result.Errors.Any();

                    foreach(var message in result.Errors)
                    {
                        validationContext.AddFailure("Password", message);
                    }
                }

                return !hasError;
            })
            .WithMessage("ignore me!"); // DO NOT CHANGE THIS MESSAGE! It's used in the pipeline to idenfity these errors
    }

    public static IRuleBuilderOptions<T, IUserMetadata> UserMetadataIsValid<T>(this IRuleBuilder<T, IUserMetadata> rule, IEnumerable<ICustomUserMetadataValidator> validators, ILocalizationService localization) where T : IUserMetadata
    {
        return rule
            .MustAsync(async (command, property, validationContext, cancellationToken) =>
            {
                bool hasError = false;

                foreach (var validator in validators)
                {
                    var result = await validator.Validate(command.Metadata);

                    hasError = hasError || result.Errors.Any();

                    foreach (var kvp in result.Errors)
                    {
                        foreach (var message in kvp.Value)
                        {
                            validationContext.AddFailure(kvp.Key, message);
                        }
                    }
                }

                return !hasError;
            })
            .WithMessage("ignore me!"); // DO NOT CHANGE THIS MESSAGE! It's used in the pipeline to idenfity these errors
    }
}