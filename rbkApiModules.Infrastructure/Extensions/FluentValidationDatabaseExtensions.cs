using FluentValidation;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Infrastructure.MediatR
{
    public static class FlutentValidatorExtensions
    {
        /// <summary>
        /// Validação de campos de ID, para verificar se o elemento existe no banco de dados
        /// </summary>
        public static IRuleBuilderOptions<T, Guid> MustExistInDatabase<T, T2>(this IRuleBuilder<T, Guid> rule, DbContext context) where T2 : BaseEntity
        {
            return rule.MustAsync(async (command, id, cancelation) => await context.Set<T2>().AnyAsync(x => x.Id == id))
                .WithMessage("'{PropertyName}' não encontrado no banco de dados");
        }

        /// <summary>
        /// Validação de campos de ID, para verificar se o elemento existe no banco de dados. O elemento pode ser nulavel
        /// </summary>
        public static IRuleBuilderOptions<T, Guid?> MustExistInDatabase<T, T2>(this IRuleBuilder<T, Guid?> rule, DbContext context) where T2 : BaseEntity
        {
            return rule.MustAsync(async (command, id, cancelation) => !id.HasValue || await context.Set<T2>().AnyAsync(x => x.Id == id.Value))
                .WithMessage("'{PropertyName}' não encontrado no banco de dados");
        }

        /// <summary>
        /// Validação de campos de ID, para verificar se o elemento existe no banco de dados em casos de campos que não podem ser nuláveis
        /// </summary>
        public static IRuleBuilderOptions<T, Guid> MustExistInDatabaseWhenNotNull<T, T2>(this IRuleBuilder<T, Guid> rule, DbContext context) where T2 : BaseEntity
        {
            return rule.MustAsync(async (command, id, cancelation) => await context.Set<T2>().AnyAsync(x => id == null || x.Id == id))
                .WithMessage("'{PropertyName}' não encontrado no banco de dados");
        }
    }
}
