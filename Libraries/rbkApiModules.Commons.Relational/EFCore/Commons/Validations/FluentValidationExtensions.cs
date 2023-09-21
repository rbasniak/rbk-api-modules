using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Relational;

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
}

public static class FluentValidationsDomainExtensions
{
    internal static IRuleBuilderOptions<TRequest, object> MustMatchAllDatabaseConstrains<TRequest>(this IRuleBuilder<TRequest, object> builder, object parentValidator)
    {
        try
        {
            var domainValidatorType = parentValidator.GetType().GetInterfaces().Single(x => x.Name == $"{nameof(IDomainEntityValidator<BaseEntity>)}`1");
            var entityType = domainValidatorType.GetGenericArguments().First();

            var localizationProperty = parentValidator.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault(x => x.FieldType == typeof(ILocalizationService));
            var localization = (ILocalizationService)localizationProperty.GetValue(parentValidator);

            var requestPropertiesToValidate = parentValidator.GetType().GetProperties().Where(x => x.GetAttribute<JsonIgnoreAttribute>() != null).ToList();

            var result = builder.SetAsyncValidator(new AsyncPredicateValidator<TRequest, object>(async (instance, property, propertyValidatorContext, cancellationToken) =>
            {
                var errors = await CheckDatabaseConstraints(parentValidator, instance, entityType, cancellationToken);

                foreach (var error in errors)
                {
                    propertyValidatorContext.AddFailure(error);
                }

                return errors.Length == 0;
            }));

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while getting reflection information from motherfucker validator in the {typeof(TRequest).Name} databse length validator", ex);
        }
    }

    private static DbContext GetDbContextFromValidator(object validator)
    {
        var property = validator.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault(x => typeof(DbContext).IsAssignableFrom(x.FieldType));

        if (property == null)
        {
            throw new SafeException($"The validator {validator.GetType().FullName.Split('.').Last().Replace("+", ".")} is expected to inject a database context", true);
        }

        var context = (DbContext)property.GetValue(validator);

        if (property == null)
        {
            throw new SafeException($"The validator {validator.GetType().FullName.Split('.').Last().Replace("+", ".")} is expected to have a non null field with the database context", true);
        }

        return context;
    }

    private static Type GetDomainValidatorEntityType(object validator)
    {
        var domainValidatorType = validator.GetType().GetInterfaces().Single(x => x.Name == $"{nameof(IDomainEntityValidator<BaseEntity>)}`1");
        var entityType = domainValidatorType.GetGenericArguments().First();

        return entityType;
    }

    private static ILocalizationService GetLocalizationFromValidator(object validator)
    {
        var property = validator.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault(x => x.FieldType == typeof(ILocalizationService));

        if (property == null)
        {
            throw new SafeException($"The validator {validator.GetType().FullName.Split('.').Last().Replace("+", ".")} is expected to inject {nameof(ILocalizationService)}", true);
        }

        var localization = (ILocalizationService)property.GetValue(validator);

        if (property == null)
        {
            throw new SafeException($"The validator {validator.GetType().FullName.Split('.').Last().Replace("+", ".")} is expected to have a non null field of type {nameof(ILocalizationService)}", true);
        }

        return localization;
    }

    private static async Task<ValidationFailure[]> CheckDatabaseConstraints(object parentValidator, object request, Type domainEntityType, CancellationToken cancellationToken)
    {
        var results = new List<ValidationFailure>();

        var entityType = GetDomainValidatorEntityType(parentValidator);

        var localization = GetLocalizationFromValidator(parentValidator);

        foreach (var requestProperty in request.GetType().GetProperties().Where(x => x.GetAttribute<JsonIgnoreAttribute>() == null))
        {
            var requestPropertyName = requestProperty.Name;

            if (requestProperty.PropertyType == typeof(Guid) && requestProperty.Name == "Id")
            {
                var entityId = (Guid)requestProperty.GetValue(request);

                var context = GetDbContextFromValidator(parentValidator);

                var entity = await context.FindAsync(domainEntityType, entityId, cancellationToken);

                if (entity == null)
                {
                    results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase)
                        .Replace("{PropertyName}", domainEntityType.Name)
                    ));
                }
                else
                {
                    if (entity is TenantEntity tenantEntity)
                    {
                        var authenticatedRequest = request as AuthenticatedRequest;

                        if (authenticatedRequest == null)
                        {
                            results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.RequestExpectedToRequireAuthentication)));
                        }
                        else
                        {
                            if (!authenticatedRequest.IsAuthenticated)
                            {
                                results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.RequestExpectedToBeAuthenticated)));
                            }
                            else
                            {
                                if (tenantEntity.TenantId != null && tenantEntity.TenantId != authenticatedRequest.Identity.Tenant)
                                {
                                    results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.PossibleUnauthorizedAccess)));
                                }
                            }
                        }
                    }
                }
            }
            else if (requestProperty.PropertyType == typeof(Guid) && requestProperty.Name.EndsWith("Id") ||
                requestProperty.PropertyType == typeof(Guid?) && requestProperty.Name.EndsWith("Id"))
            {
                Guid entityId;

                if (requestProperty.PropertyType == typeof(Guid))
                {
                    entityId = (Guid)requestProperty.GetValue(request);
                }
                else
                {
                    var propertyValue = (Guid?)requestProperty.GetValue(request);

                    if (propertyValue != null)
                    {
                        entityId = propertyValue.Value;
                    }
                    else
                    {
                        continue;
                    }
                }

                var entityProperty = entityType.GetProperty(requestPropertyName.Substring(0, requestPropertyName.Length - 2));

                if (entityProperty == null)
                {
                    continue;
                }

                var context = GetDbContextFromValidator(parentValidator);

                var entity = await context.FindAsync(entityProperty.PropertyType, entityId, cancellationToken);

                if (entity == null)
                {
                    results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.EntityNotFoundInDatabase)
                        .Replace("{PropertyName}", entityProperty.Name)
                    ));
                }
                else
                {
                    if (entity is TenantEntity tenantEntity)
                    {
                        var authenticatedRequest = request as AuthenticatedRequest;

                        if (authenticatedRequest == null)
                        {
                            results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.RequestExpectedToRequireAuthentication)));
                        }
                        else
                        {
                            if (!authenticatedRequest.IsAuthenticated)
                            {
                                results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.RequestExpectedToBeAuthenticated)));
                            }
                            else
                            {
                                if (tenantEntity.TenantId != null && tenantEntity.TenantId != authenticatedRequest.Identity.Tenant)
                                {
                                    results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.PossibleUnauthorizedAccess)));
                                }
                            }
                        }
                    } 
                }
            }
            else if (requestProperty.PropertyType == typeof(String))
            {
                var entityProperty = entityType.GetProperty(requestPropertyName);

                if (entityProperty == null)
                {
                    continue;
                }

                var minLengthAttribute = entityProperty.GetAttribute<MinLengthAttribute>();
                var maxLengthAttribute = entityProperty.GetAttribute<MaxLengthAttribute>();
                var requiredAttribute = entityProperty.GetAttribute<RequiredAttribute>();
                var dialogData = entityProperty.GetAttribute<DialogDataAttribute>();

                var isRequired = requiredAttribute != null;
                var hasMinLengthRequirement = minLengthAttribute != null; // TODO: must be mandatory for strings
                var hasMaxLengthRequirement = maxLengthAttribute != null; // TODO: must be mandatory for strings

                var minLength = minLengthAttribute.Length;
                var maxLength = maxLengthAttribute.Length;

                if (dialogData != null)
                {
                    requestPropertyName = dialogData.Name;
                }

                var propertyValue = requestProperty.GetValue(request) ?? "";

                if (isRequired && String.IsNullOrEmpty(propertyValue.ToString()))
                {
                    // IsRequired failed, pass a validator that always fail

                    results.Add(new ValidationFailure(requestPropertyName, $"The field '{requestPropertyName}' is required"));

                    continue;
                }

                if (hasMinLengthRequirement && hasMaxLengthRequirement && minLength != maxLength)
                {
                    if (propertyValue.ToString().Length < minLength || propertyValue.ToString().Length > maxLength)
                    {
                        results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.FieldMustHaveLengthBetweenMinAndMax)
                            .Replace("{PropertyName}", requestPropertyName)
                            .Replace("{0}", minLength.ToString())
                            .Replace("{1}", maxLength.ToString())
                        ));
                    }
                }
                else if (hasMinLengthRequirement && hasMaxLengthRequirement && minLength == maxLength)
                {
                    if (propertyValue.ToString().Length != minLength)
                    {
                        results.Add(new ValidationFailure(requestPropertyName, localization.LocalizeString(SharedValidationMessages.Common.FieldMustHaveFixedLength)
                            .Replace("{0}", minLength.ToString())
                        ));
                    }
                }
            }
        }

        return results.ToArray();
    }
}
