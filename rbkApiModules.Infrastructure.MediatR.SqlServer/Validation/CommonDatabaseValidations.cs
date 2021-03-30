using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    public class CommonDatabaseValidations: ICommonDatabaseValidations
    {
        public ValidationResult[] ValidateExistingDbElements(IHttpContextAccessor httpContextAccessor, object command)
        {
            var context = httpContextAccessor.HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            var commandType = command.GetType();
            var properties = commandType.GetProperties().ToList();

            var results = new List<ValidationResult>();

            foreach (var property in properties)
            {
                var validationResult = new ValidationResult(property.Name);

                var attributes = property.GetCustomAttributes(true).ToList();

                foreach (var attribute in attributes)
                {
                    if (attribute is MustExistAttribute existingEntityAttribute)
                    {
                        var propertyValue = property.GetValue(command);

                        if (property.PropertyType == typeof(Guid))
                        {
                            var result = LoadEntityFromDatabase(context, (Guid)property.GetValue(command), existingEntityAttribute.EntityType);

                            if (result == null)
                            {
                                validationResult.SetEntityNotFound();
                            }
                        }
                        else if (property.PropertyType == typeof(Guid[]))
                        {
                            var elementList = propertyValue as Guid[];

                            if (elementList == null)
                            {
                                validationResult.SetEmptyEntityList();
                            }
                            else if (elementList.Length == 0)
                            {
                                validationResult.SetEmptyEntityList();
                            }
                            else
                            {
                                var notFoundElements = new List<Guid>();
                                foreach (var id in elementList)
                                {
                                    var result = LoadEntityFromDatabase(context, id, existingEntityAttribute.EntityType);
                                    if (result == null)
                                    {
                                        notFoundElements.Add(id);
                                    }
                                }

                                if (notFoundElements.Count > 0)
                                {
                                    validationResult.SetMultipleEntitiesNotFound();
                                }
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("Only Guid and Guid[] types are supported by ExistingEntityAttribute");
                        }
                    }
                }

                if (validationResult.HasErrors)
                {
                    results.Add(validationResult);
                }
            }

            return results.ToArray();
        }

        public ValidationResult[] ValidateNonUsedDbElements(IHttpContextAccessor httpContextAccessor, object command)
        {
            var context = httpContextAccessor.HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            var results = new List<ValidationResult>();

            var isUsed = false;

            foreach (var commandProperty in command.GetType().GetProperties())
            {
                if (commandProperty.Name == "Id")
                {
                    var existingEntityAttribute = commandProperty.GetCustomAttribute<MustExistAttribute>();
                    var nonUsedEntityAttribute = commandProperty.GetCustomAttribute<MustNotBeUsedAttribute>();
                    
                    if (nonUsedEntityAttribute != null && existingEntityAttribute == null)
                    {
                        throw new Exception("The attribute 'NonUsedEntity' must be used paired with 'ExistingEntity'");
                    }

                    if (nonUsedEntityAttribute != null && existingEntityAttribute != null)
                    {
                        var tableType = existingEntityAttribute.EntityType;
                        var tableProperties = tableType.GetProperties().ToList();

                        var id = GetIdValueIfExist(command, command.GetType().GetProperties().ToList());

                        if (id == null)
                        {
                            throw new Exception("The attribute 'NonUsedEntity' can only be used with commands that have an 'Id' property");
                        }

                        var entity = LoadEntityFromDatabase(context, id.Value, existingEntityAttribute.EntityType);

                        // If the entity doesn't exist in database, it's this validator's responsability to validate it.
                        if (entity == null) return new ValidationResult[0];

                        foreach (var property in tableProperties)
                        {
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                if (property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments.First().IsClass)
                                {
                                    context.Entry(entity).Collection(property.Name).Load();

                                    var value = property.GetValue(entity);

                                    var enumerable = value as IEnumerable;

                                    if (enumerable != null)
                                    {
                                        var enumerator = enumerable.GetEnumerator();
                                        
                                        if (enumerator.MoveNext())
                                        {
                                            isUsed = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (isUsed)
                        {
                            break;
                        }
                    }
                }
            }

            if (isUsed)
            {
                var validationResult = new ValidationResult("");
                validationResult.SetUsedEntity();

                results.Add(validationResult);
            }

            return results.ToArray();
        }

        public ValidationResult[] ValidateIsUniqueDbElements(IHttpContextAccessor httpContextAccessor, object command)
        {
            var context = httpContextAccessor.HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            var commandType = command.GetType();
            var properties = commandType.GetProperties().ToList();

            var results = new List<ValidationResult>();

            foreach (var property in properties)
            {
                var validationResult = new ValidationResult(property.Name);

                var attributes = property.GetCustomAttributes(true).ToList();

                foreach (var attribute in attributes)
                {
                    if (attribute is MustBeUniqueAttribute isUniqueAttribute)
                    {
                        var propertyValue = property.GetValue(command);

                        if (property.PropertyType == typeof(string))
                        {
                            var result = CheckPropertyValueExistOnDatabase(context, (string)property.GetValue(command), isUniqueAttribute.Name, isUniqueAttribute.EntityType, GetIdValueIfExist(command, properties));

                            if (result)
                            {
                                validationResult.SetEntityNotUnique();
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("Only String types are supported by isUniqueAttribute");
                        }
                    }
                }

                if (validationResult.HasErrors)
                {
                    results.Add(validationResult);
                }
            }

            return results.ToArray();
        }

        private Guid? GetIdValueIfExist(object command, List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (property.Name == "Id" && property.PropertyType == typeof(Guid))
                {
                    return (Guid)property.GetValue(command);
                }
            }

            return null;
        }

        private bool CheckPropertyValueExistOnDatabase(object context, string propertyValue, string propertyName, Type entityType, Guid? Id)
        {
            var setMethod = context.GetType().GetMethod("Set", new Type[0]);

            var genericSetMethod = setMethod.MakeGenericMethod(entityType);
            var dbsetInstance = genericSetMethod.Invoke(context, new object[0]);

            var whereMethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Where")
                .First().MakeGenericMethod(entityType);

            var result = (IQueryable<object>)whereMethod.Invoke(null, new object[] { dbsetInstance, BuildWhereLambda(entityType, propertyName, propertyValue, Id) });

            return result.Any();
        }

        private LambdaExpression BuildWhereLambda(Type parameterType, string propertyName, string value, Guid? Id)
        {
            var parameter = Expression.Parameter(parameterType);

            if(Id.HasValue)
            {
                var body = Expression.AndAlso(
                    Expression.Equal(
                        Expression.PropertyOrField(parameter, propertyName),
                        Expression.Constant(value)
                    ),
                    Expression.NotEqual(
                        Expression.PropertyOrField(parameter, "Id"),
                        Expression.Constant(Id.Value)
                    )
                );

                return Expression.Lambda(body, parameter);
            }
            else
            {
                var memberExpression = Expression.Property(parameter, propertyName);
                var equalsTo = Expression.Constant(value);
                var equality = Expression.Equal(memberExpression, equalsTo);

                return Expression.Lambda(equality, parameter);
            }
        }

        private object LoadEntityFromDatabase(object context, Guid id, Type type)
        {
            var setMethod = context.GetType().GetMethod("Set", new Type[0]);

            var genericSetMethod = setMethod.MakeGenericMethod(type);
            var dbsetInstance = genericSetMethod.Invoke(context, new object[0]);

            var dbsetType = dbsetInstance.GetType();
            var findMethod = dbsetType.GetMethod("Find");

            var result = findMethod.Invoke(dbsetInstance, new object[] { new object[] { id } });

            return result;
        }
    }
}
