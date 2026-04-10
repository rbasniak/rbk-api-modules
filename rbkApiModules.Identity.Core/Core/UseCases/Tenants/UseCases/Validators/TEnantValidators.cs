namespace rbkApiModules.Identity.Core;

internal static class TenantValidators
{
    public static IRuleBuilderOptions<T, AuthenticatedUser> TenantExistOnDatabase<T>(this IRuleBuilder<T, AuthenticatedUser> rule, ITenantsService tenants, ILocalizationService localization)
    {
        return rule
            .MustAsync(async (identity, cancellationToken) =>
            {
                if (String.IsNullOrEmpty(identity.Tenant))
                {
                    return true;
                }
                else
                {
                    return await tenants.FindAsync(identity.Tenant, cancellationToken) != null;
                }
            })
            .WithMessage(command => localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound));
    }
}

