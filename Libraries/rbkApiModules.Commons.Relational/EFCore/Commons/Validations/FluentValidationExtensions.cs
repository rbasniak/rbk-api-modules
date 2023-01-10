using FluentValidation;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Commons.Relational;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustExistInDatabase<T, T2>(this IRuleBuilder<T, Guid> rule, DbContext context) where T2 : BaseEntity
    {
        return rule.MustAsync(async (command, id, cancelation) => await context.Set<T2>().AnyAsync(x => x.Id == id))
            .WithMessage("'{PropertyName}' not found in database");
    }
}
