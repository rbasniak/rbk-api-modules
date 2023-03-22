using FluentValidation;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Commons.Relational;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistInDatabase<T, T2>(this IRuleBuilder<T, Guid> rule, DbContext context, ILocalizationService localization) where T2 : BaseEntity
    {
        return rule.MustAsync(async (command, id, cancelation) => await context.Set<T2>().AnyAsync(x => x.Id == id))
            .WithMessage(localization.GetValue(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }

    public static IRuleBuilderOptions<T, Guid?> MustExistInDatabaseWhenNotNull<T, T2>(this IRuleBuilder<T, Guid?> rule, DbContext context, ILocalizationService localization) where T2 : BaseEntity
    {
        return rule.MustAsync(async (command, id, cancelation) => id == null || await context.Set<T2>().AnyAsync(x => x.Id == id.Value))
            .WithMessage(localization.GetValue(SharedValidationMessages.Common.EntityNotFoundInDatabase));
    }
}
