using FluentValidation;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Commons.Core;

public static class FluentValidationExtensions
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
    public static IRuleBuilderOptions<T, string> IsRequired<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .NotEmpty().WithMessage(localization.GetValue("O campo '{PropertyName}' não pode ser vazio"));
    }

    /// <summary>
    /// Validação de campos de texto que não podem ser nulos
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustNotBeNull<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .NotNull().WithMessage(localization.GetValue("O campo '{PropertyName}' não pode ser nulo"));
    }

    /// <summary>
    /// Validação de campos numéricos que devem ficar em um determinado range
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustHasLengthBetween<T>(this IRuleBuilder<T, string> rule, int min, int max, ILocalizationService localization)
    {
        return rule
            .Must(x => String.IsNullOrEmpty(x) || (x.Length >= min && x.Length <= max))
            .WithMessage(localization.GetValue("O campo '{PropertyName}' deve conter entre " + min + " e " + max + " caracteres"));
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
    /// Validação de campos de e-mail
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeEmail<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .EmailAddress().WithMessage(localization.GetValue("E-mail com formato inválido"));
    }
}