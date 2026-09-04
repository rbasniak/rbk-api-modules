using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Core.Validation;

public static class FluentValidationBasicExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistInDatabase<T, T2>(this IRuleBuilder<T, Guid> rule, DbContext context, ILocalizationService localization) where T2 : BaseEntity
    {
        return rule.MustAsync(async (command, id, cancelation) => await context.Set<T2>().AnyAsync(x => x.Id == id))
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }

    public static IRuleBuilderOptions<T, Guid?> MustExistInDatabaseWhenNotNull<T, T2>(this IRuleBuilder<T, Guid?> rule, DbContext context, ILocalizationService localization) where T2 : BaseEntity
    {
        return rule.MustAsync(async (command, id, cancelation) => id == null || await context.Set<T2>().AnyAsync(x => x.Id == id.Value))
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }

    public static IRuleBuilderOptions<T, Guid> MustExistInDatabaseForCurrentTenant<T, TEntity>(this IRuleBuilder<T, Guid> rule, DbContext context, ILocalizationService localization)
        where T : AuthenticatedRequest
        where TEntity : TenantEntity
    {
        return rule.MustAsync(async (command, id, cancellationToken) =>
                await ExistsForCurrentTenantAsync<TEntity>(command, id, context, cancellationToken))
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }

    public static IRuleBuilderOptions<T, Guid?> MustExistInDatabaseForCurrentTenantWhenNotNull<T, TEntity>(this IRuleBuilder<T, Guid?> rule, DbContext context, ILocalizationService localization)
        where T : AuthenticatedRequest
        where TEntity : TenantEntity
    {
        return rule.MustAsync(async (command, id, cancellationToken) =>
                id == null || await ExistsForCurrentTenantAsync<TEntity>(command, id.Value, context, cancellationToken))
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }

    private static async Task<bool> ExistsForCurrentTenantAsync<TEntity>(
        AuthenticatedRequest command,
        Guid id,
        DbContext context,
        CancellationToken cancellationToken)
        where TEntity : TenantEntity
    {
        if (command.Identity.Tenant is null)
        {
            return false;
        }

        if (command.Identity.Tenant == String.Empty)
        {
            return await context.Set<TEntity>().AnyAsync(x => x.Id == id && String.IsNullOrEmpty(x.TenantId), cancellationToken);
        }

        return await context.Set<TEntity>().AnyAsync(x => x.Id == id && x.TenantId == command.Identity.Tenant, cancellationToken);
    }
}