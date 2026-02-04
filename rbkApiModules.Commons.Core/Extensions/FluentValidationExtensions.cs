using FluentValidation;
using System.ComponentModel;
using System.Net.Mail;

namespace rbkApiModules.Commons.Core;

public static class FluentValidationExtensions
{
    public const string ONLY_NUMBERS_REGEX = @"^[0-9]+$";

    /// <summary>
    /// Validação de campos de texto que devem ser Ids de banco.
    /// Retorna válido se o Id for vazio ou nulo, precisa usar o MustNotBeWmpty 
    /// antes desse validador caso o id não possa ser nulo
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeValidId<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .Must(value => !String.IsNullOrEmpty(value) ? Guid.TryParse(value, out Guid result) : true)
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.InvalidIdFormat));
    }

    public static IRuleBuilderOptions<T, string> IsRequired<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .NotEmpty().WithMessage(localization.LocalizeString(SharedValidationMessages.Common.FieldCannotBeEmpty));
    }

    public static IRuleBuilderOptions<T, string> MustNotBeNull<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .NotNull().WithMessage(localization.LocalizeString(SharedValidationMessages.Common.FieldCannotBeNull));
    }

    public static IRuleBuilderOptions<T, string> MustHasLengthBetween<T>(this IRuleBuilder<T, string> rule, int min, int max, ILocalizationService localization)
    {
        return rule
            .Must(x => String.IsNullOrEmpty(x) || (x.Length >= min && x.Length <= max))
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.FieldMustHaveLengthBetweenMinAndMax).Replace("{0}", min.ToString()).Replace("{1}", max.ToString()));
    }

    public static IRuleBuilderOptions<T, string[]> ListMustHaveItems<T>(this IRuleBuilder<T, string[]> rule)
    {
        return rule
            .Must(x => x != null && x.Length != 0);
    }

    public static IRuleBuilderOptions<T, string> MustBeEmail<T>(this IRuleBuilder<T, string> rule, ILocalizationService localization)
    {
        return rule
            .Must(x =>
            {
                if (!MailAddress.TryCreate(x, out var mailAddress))
                {
                    return false;
                }

                var hostParts = mailAddress.Host.Split('.');

                if (hostParts.Length == 1)
                {
                    return false; // No dot.
                }

                if (hostParts.Any(p => p == string.Empty))
                {
                    return false; // Double dot.
                }

                if (hostParts[^1].Length < 2)
                {
                    return false; // TLD only one letter.
                }

                if (mailAddress.User.Contains(' '))
                {
                    return false;
                }

                if (mailAddress.User.Split('.').Any(p => p == string.Empty))
                {
                    return false; // Double dot or dot at end of user part.
                }

                return true;
            })
            .WithMessage(localization.LocalizeString(SharedValidationMessages.Common.InvalidEmailFormat));
    }
}

public class SharedValidationMessages : ILocalizedResource
{
    public enum Common
    {
        [Description("The field '{PropertyName}' cannot be empty")] FieldCannotBeEmpty,
        [Description("The field '{PropertyName}' already exists in the database")] FieldValueAlreadyUsedInDatabase,
        [Description("The field '{PropertyName}' cannot be null")] FieldCannotBeNull,
        [Description("The field '{PropertyName}' must have between {0} and {1} characters")] FieldMustHaveLengthBetweenMinAndMax,
        [Description("The field '{PropertyName}' must have {0} characters")] FieldMustHaveFixedLength,
        [Description("E-mail is invalid")] InvalidEmailFormat,
        [Description("Id is invalid")] InvalidIdFormat,
        [Description("'{PropertyName}' not found in database")] EntityNotFoundInDatabase,
        [Description("Request expected to require authentication")] RequestExpectedToRequireAuthentication,
        [Description("Request expected to be authenticated")] RequestExpectedToBeAuthenticated,
        [Description("Possible unauthorized access")] PossibleUnauthorizedAccess,
    }

    public enum Errors
    {
        [Description("Internal server error")] InternalServerError,
    }
}