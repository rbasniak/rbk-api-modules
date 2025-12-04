using rbkApiModules.Identity.Core;

internal static class RolesValidators
{
        public static IRuleBuilderOptions<T, Guid> RoleExistOnDatabaseForTheCurrentTenant<T>(this IRuleBuilder<T, Guid> rule, IRolesService roles, ILocalizationService localization) where T : AuthenticatedRequest
    {
        return rule
            .MustAsync(async (command, roleId, cancellationToken) =>
            {
                var role = await roles.FindAsync(roleId, cancellationToken);

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
            .WithMessage(command => localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound));
    }

}