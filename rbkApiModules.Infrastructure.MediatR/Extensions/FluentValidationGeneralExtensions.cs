using FluentValidation;
using FluentValidation.Validators;
using System;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Extensões para FluentValidation
    /// </summary>
    public static class FluentValidationGeneralExtensions
    {
        public const string ONLY_NUMBERS_REGEX = @"^[0-9]+$";

        /// <summary>
        /// Validação de campos de texto que devem ser Ids de banco.
        /// Retorna válido se o Id for vazio ou nulo, precisa usar o MustNotBeWmpty 
        /// antes desse validador caso o id não possa ser nulo
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeValidId<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .Must(value => !String.IsNullOrEmpty(value) ? Guid.TryParse(value, out Guid result) : true)
                .WithMessage("Id com formato inválido");
        }

        /// <summary>
        /// Validação de campos de texto que não podem ser vazios
        /// </summary>
        public static IRuleBuilderOptions<T, string> IsRequired<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("O campo '{PropertyName}' não pode ser vazio");
        }

        /// <summary>
        /// Validação de campos de texto que não podem ser nulos
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustNotBeNull<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser nulo");
        }

        /// <summary>
        /// Validação de campos numéricos que devem ficar em um determinado range
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustHasLengthBetween<T>(this IRuleBuilder<T, string> rule, int min, int max)
        {
            return rule
                .Length(min, max).WithMessage("O campo '{PropertyName}' deve conter entre " + min + " e " + max + " caracteres");
        }

        /// <summary>
        /// Validação de campos de listas de texto que devem conter ao menos um item na lista
        /// </summary>
        public static IRuleBuilderOptions<T, string[]> ListMustHaveItems<T>(this IRuleBuilder<T, string[]> rule)
        {
            return rule
                .Must(x => x != null && x.Length != 0);
        }

        /// <summary>
        /// Validação de campos de telefone
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBePhone<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .MustHasLengthBetween(10, 11)
                .SetValidator(new RegularExpressionValidator(ONLY_NUMBERS_REGEX)).WithMessage("Telefone com formato inválido")
                .When((model, prop) => !String.IsNullOrEmpty(prop));
        }

        /// <summary>
        /// Validação de campos de e-mail
        /// </summary>
        public static IRuleBuilderOptions<T, string> MustBeEmail<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .EmailAddress().WithMessage("E-mail com formato inválido");
        }

        /// <summary>
        /// Extensão para auxiliar nos outros métodos de extensão de validação
        /// </summary>
        private static IRuleBuilderOptions<T, TProperty> When<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            return rule.Configure(config =>
            {
                config.ApplyCondition(ctx => predicate((T)ctx.InstanceToValidate, (TProperty)ctx.PropertyValue), applyConditionTo);
            });
        }
    }
}
