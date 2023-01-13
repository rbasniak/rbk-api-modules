﻿using FluentValidation;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

internal static class Validators
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
            .WithErrorCode(ValidationErrorCodes.UNAUTHORIZED)
            .WithMessage(command => localization.GetValue("Unauthorized access"));
    }

    public static IRuleBuilderOptions<T, AuthenticatedUser> TenantExistOnDatabase<T>(this IRuleBuilder<T, AuthenticatedUser> rule, ITenantsService tenants, ILocalizationService localization)
    {
        return rule
            .MustAsync(async (identity, cancellation) =>
            {
                if (String.IsNullOrEmpty(identity.Tenant))
                {
                    return true;
                }
                else
                {
                    return await tenants.FindAsync(identity.Tenant, cancellation) != null;
                }
            })
            .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
            .WithMessage(command => localization.GetValue("Tenant not found"));
    }

    public static IRuleBuilderOptions<T, Guid> RoleExistOnDatabaseForTheCurrentTenant<T>(this IRuleBuilder<T, Guid> rule, IRolesService roles, ILocalizationService localization) where T: AuthenticatedCommand
    {
        return rule
            .MustAsync(async (command, roleId, cancellation) =>
            {
                var role = await roles.FindAsync(roleId, cancellation);

                if (role == null)
                {
                    return false;
                }
                else
                {
                    if (String.IsNullOrEmpty(command.Identity.Tenant))
                    {
                        return String.IsNullOrEmpty(role.TenantId);
                    }
                    else
                    {
                        return role.TenantId == command.Identity.Tenant;
                    }
                }

            })
            .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
            .WithMessage(command => localization.GetValue("Role not found"));
    }

    public static IRuleBuilderOptions<T, Guid> ClaimExistOnDatabase<T>(this IRuleBuilder<T, Guid> rule, IClaimsService claims, ILocalizationService localization)
    {
        return rule
            .MustAsync(async (claimId, cancellation) =>
            {
                return await claims.FindAsync(claimId, cancellation) != null;
            })
            .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
            .WithMessage(command => localization.GetValue("Claim not found"));
    }

    public static IRuleBuilderOptions<T, string> PasswordPoliciesAreValid<T>(this IRuleBuilder<T, string> rule, IEnumerable<ICustomPasswordPolicyValidator> validators, ILocalizationService localization)  
    {
        return rule
            .MustAsync(async (command, password, validationContext, cancellation) =>
            {
                bool hasError = false;

                foreach (var validator in validators)
                {
                    var result = await validator.Validate(password);

                    hasError = hasError || result.HasErrors;

                    foreach (var message in result.Errors)
                    {
                        validationContext.AddFailure(localization.GetValue(message));
                    }
                }

                return !hasError;
            })
            .WithMessage("none");
    }

    public static IRuleBuilderOptions<T, ILoginData> LoginPoliciesAreValid<T>(this IRuleBuilder<T, ILoginData> rule, IEnumerable<ICustomLoginPolicyValidator> validators, ILocalizationService localization) where T: ILoginData
    {
        return rule
            .MustAsync(async (command, property, validationContext, cancellation) =>
            {
                bool hasError = false;

                foreach (var validator in validators)
                {
                    var result = await validator.Validate(command.Tenant, command.Username, command.Password);

                    hasError = hasError || result.HasErrors;

                    foreach (var message in result.Errors)
                    {
                        validationContext.AddFailure(localization.GetValue(message));
                    }
                }

                return !hasError;
            })
            .WithMessage("none");
    }
}