using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

            if (requestProperty.PropertyType == typeof(Guid) && requestPropertyName == "Id")
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
                var uniqueInTenantAttribute = entityProperty.GetAttribute<UniqueInTenantAttribute>();
                var uniqueInApplicationAttribute = entityProperty.GetAttribute<UniqueInApplicationAttribute>();

                var dialogData = entityProperty.GetAttribute<DialogDataAttribute>();

                var isRequired = requiredAttribute != null;
                var hasMinLengthRequirement = minLengthAttribute != null; // TODO: must be mandatory for strings
                var hasMaxLengthRequirement = maxLengthAttribute != null; // TODO: must be mandatory for strings
                var isUniqueInTenant = uniqueInTenantAttribute != null;
                var isUniqueInApplication = uniqueInApplicationAttribute != null;

                if (dialogData != null)
                {
                    requestPropertyName = dialogData.Name;
                }

                if (isUniqueInTenant && isUniqueInApplication)
                {
                    throw new SafeException($"{domainEntityType.GetType().Name}.{requestPropertyName} uses both IsUniqueInTenant and IsUniqueInApplication. Choose one or another");
                }

                if (!hasMinLengthRequirement || !hasMaxLengthRequirement)
                {
                    throw new SafeException($"{domainEntityType.GetType().Name}.{requestPropertyName} is of type String and must have minimum and maximum lenghts specified");
                }

                var minLength = minLengthAttribute.Length;
                var maxLength = maxLengthAttribute.Length;

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

                if (isUniqueInApplication)
                {
                    var idProperty = request.GetType().GetProperty(nameof(BaseEntity.Id));

                    bool isCreation = idProperty == null;
                    bool isUpdate = idProperty != null;

                    var context = GetDbContextFromValidator(parentValidator);

                    if (isCreation)
                    {
                        var isValueAlreayUsed = await IsValueUsedInApplicationAsync(requestPropertyName, (string)propertyValue, domainEntityType, context, cancellationToken);

                        if (isValueAlreayUsed)
                        {
                            results.Add(new ValidationFailure(requestPropertyName,
                                localization.LocalizeString(SharedValidationMessages.Common.FieldValueAlreadyUsedInDatabase).Replace("{PropertyName}", requestProperty.Name)));
                        }
                    }

                    if (isUpdate)
                    {
                        var entityId = (Guid)idProperty.GetValue(request);

                        var exists = await IsValueUsedInApplicationAsync(entityId, requestPropertyName, (string)propertyValue, domainEntityType, context, cancellationToken);

                        if (exists)
                        {
                            results.Add(new ValidationFailure(requestPropertyName,
                                localization.LocalizeString(SharedValidationMessages.Common.FieldValueAlreadyUsedInDatabase).Replace("{PropertyName}", requestProperty.Name)));
                        }
                    }
                }

                if (isUniqueInTenant)
                {
                    var idProperty = request.GetType().GetProperty(nameof(BaseEntity.Id));

                    if (!domainEntityType.IsAssignableTo(typeof(TenantEntity)))
                    {
                        throw new SafeException($"The property {domainEntityType.Name}.{requestPropertyName} must me unique in the tenant, but the entity does not inherit from TenantEntity");
                    }

                    var authenticatedRequest = request as AuthenticatedRequest;

                    if (authenticatedRequest == null)
                    {
                        throw new SafeException($"The request is expected to require authentication (because the property {domainEntityType.Name}.{requestPropertyName} must me unique in the tenant) and it does not inherit from AuthenticatedRequest");
                    }

                    bool isCreation = idProperty == null;
                    bool isUpdate = idProperty != null;

                    var context = GetDbContextFromValidator(parentValidator);

                    if (isCreation)
                    {
                        var isValueAlreayUsed = await IsValueUsedInTenantAsync(authenticatedRequest.Identity.Tenant, requestPropertyName, (string)propertyValue, domainEntityType, context, cancellationToken);

                        if (isValueAlreayUsed)
                        {
                            results.Add(new ValidationFailure(requestPropertyName,
                                localization.LocalizeString(SharedValidationMessages.Common.FieldValueAlreadyUsedInDatabase).Replace("{PropertyName}", requestProperty.Name)));
                        }
                    }

                    if (isUpdate)
                    {
                        var entityId = (Guid)idProperty.GetValue(request);

                        var isValueAlreayUsed = await IsValueUsedInTenantAsync(entityId, authenticatedRequest.Identity.Tenant, requestPropertyName, (string)propertyValue, domainEntityType, context, cancellationToken);

                        if (isValueAlreayUsed)
                        {
                            results.Add(new ValidationFailure(requestPropertyName,
                                localization.LocalizeString(SharedValidationMessages.Common.FieldValueAlreadyUsedInDatabase).Replace("{PropertyName}", requestProperty.Name)));
                        }
                    }
                }
            }
        }

        return results.ToArray();
    }

    private static async Task<bool> IsValueUsedInApplicationAsync(Guid entityId, string propertyName, string propertyValue, Type entityType, DbContext context, CancellationToken cancellationToken )
    {
        var setMethod = context.GetType().GetMethods().Where(x => x.Name == "Set" && x.GetParameters().Count() == 0).Single();

        var genericSetMethod = setMethod.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(context, null);

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyName);

        var desiredPropertyExpressionConstant = Expression.Constant(propertyValue);
        var desiredPropertyEqualExpression = Expression.Equal(property, desiredPropertyExpressionConstant);

        LambdaExpression lambda;

        var idPropertyExpression = Expression.Property(parameter, nameof(BaseEntity.Id));
        var idConstantExpression = Expression.Constant(entityId);
        var idEqualExpression = Expression.NotEqual(idPropertyExpression, idConstantExpression);

        var combinedExpression = Expression.AndAlso(desiredPropertyEqualExpression, idEqualExpression);
        lambda = Expression.Lambda(combinedExpression, parameter);

        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Single(x => x.Name == "AnyAsync" && x.GetParameters().Count() == 3);
        var anyAsyncGenericMethod = anyAsyncMethod.MakeGenericMethod(entityType);
        var exists = await(Task<bool>)anyAsyncGenericMethod.Invoke(null, new object[] { dbSet, lambda, cancellationToken });

        return exists;
    }

    private static async Task<bool> IsValueUsedInApplicationAsync(string propertyName, string propertyValue, Type entityType, DbContext context, CancellationToken cancellationToken)
    {
        var setMethod = context.GetType().GetMethods().Where(x => x.Name == "Set" && x.GetParameters().Count() == 0).Single();

        var genericSetMethod = setMethod.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(context, null);

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyName);

        var desiredPropertyExpressionConstant = Expression.Constant(propertyValue);
        var desiredPropertyEqualExpression = Expression.Equal(property, desiredPropertyExpressionConstant);

        var lambda = Expression.Lambda(desiredPropertyEqualExpression);

        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Single(x => x.Name == "AnyAsync" && x.GetParameters().Count() == 3);
        var anyAsyncGenericMethod = anyAsyncMethod.MakeGenericMethod(entityType);
        var exists = await (Task<bool>)anyAsyncGenericMethod.Invoke(null, new object[] { dbSet, lambda, cancellationToken });

        return exists;
    }

    private static async Task<bool> IsValueUsedInTenantAsync(Guid entityId, string tenant, string propertyName, string propertyValue, Type entityType, DbContext context, CancellationToken cancellationToken)
    {
        var setMethod = context.GetType().GetMethods().Where(x => x.Name == "Set" && x.GetParameters().Count() == 0).Single();

        var genericSetMethod = setMethod.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(context, null);

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyName);

        var desiredPropertyExpressionConstant = Expression.Constant(propertyValue);
        var desiredPropertyEqualExpression = Expression.Equal(property, desiredPropertyExpressionConstant);

        var tenantPropertyExpression = Expression.Property(parameter, nameof(TenantEntity.TenantId));
        var tenantConstantExpression = Expression.Constant(tenant);
        var tenantEqualExpression = Expression.Equal(tenantPropertyExpression, tenantConstantExpression);

        LambdaExpression lambda;

        var idPropertyExpression = Expression.Property(parameter, nameof(BaseEntity.Id));
        var idConstantExpression = Expression.Constant(entityId);
        var idEqualExpression = Expression.NotEqual(idPropertyExpression, idConstantExpression);

        var propertyAndIdCombinedExpression = Expression.AndAlso(desiredPropertyEqualExpression, idEqualExpression);

        var propertyAndValueAndTenantCombinedExpression = Expression.AndAlso(propertyAndIdCombinedExpression, tenantEqualExpression);

        lambda = Expression.Lambda(propertyAndValueAndTenantCombinedExpression, parameter);

        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Single(x => x.Name == "AnyAsync" && x.GetParameters().Count() == 3);
        var anyAsyncGenericMethod = anyAsyncMethod.MakeGenericMethod(entityType);
        var exists = await (Task<bool>)anyAsyncGenericMethod.Invoke(null, new object[] { dbSet, lambda, cancellationToken });

        return exists;
    }

    private static async Task<bool> IsValueUsedInTenantAsync(string tenant, string propertyName, string propertyValue, Type entityType, DbContext context, CancellationToken cancellationToken)
    {
        var setMethod = context.GetType().GetMethods().Where(x => x.Name == "Set" && x.GetParameters().Count() == 0).Single();

        var genericSetMethod = setMethod.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(context, null);

        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyName);

        var desiredPropertyExpressionConstant = Expression.Constant(propertyValue);
        var desiredPropertyEqualExpression = Expression.Equal(property, desiredPropertyExpressionConstant);

        var tenantPropertyExpression = Expression.Property(parameter, nameof(TenantEntity.TenantId));
        var tenantConstantExpression = Expression.Constant(tenant);
        var tenantEqualExpression = Expression.Equal(tenantPropertyExpression, tenantConstantExpression);

        LambdaExpression lambda;

        var propertyAndTenantCombinedExpression = Expression.AndAlso(desiredPropertyEqualExpression, tenantEqualExpression);

        lambda = Expression.Lambda(propertyAndTenantCombinedExpression, parameter);

        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Single(x => x.Name == "AnyAsync" && x.GetParameters().Count() == 3);
        var anyAsyncGenericMethod = anyAsyncMethod.MakeGenericMethod(entityType);
        var exists = await (Task<bool>)anyAsyncGenericMethod.Invoke(null, new object[] { dbSet, lambda, cancellationToken });

        return exists;
    }
}
