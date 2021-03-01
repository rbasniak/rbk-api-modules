﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.Models;
using System;
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
        private object LoadEntityFromDatabase(object context, Guid id, ExistingEntityAttribute exstingEntityAttribute)
        {
            var setMethod = context.GetType().GetMethod("Set", new Type[0]);

            var genericSetMethod = setMethod.MakeGenericMethod(exstingEntityAttribute.EntityType);
            var dbsetInstance = genericSetMethod.Invoke(context, new object[0]);

            var dbsetType = dbsetInstance.GetType();
            var findMethod = dbsetType.GetMethod("Find");

            var result = findMethod.Invoke(dbsetInstance, new object[] { new object[] { id } });

            return result;
        }

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
                    if (attribute is ExistingEntityAttribute existingEntityAttribute)
                    {
                        var propertyValue = property.GetValue(command);

                        if (property.PropertyType == typeof(Guid))
                        {
                            var result = LoadEntityFromDatabase(context, (Guid)property.GetValue(command), existingEntityAttribute);

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
                                    var result = LoadEntityFromDatabase(context, id, existingEntityAttribute);
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
                    if (attribute is IsUniqueAttribute isUniqueAttribute)
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

            var result = (IQueryable<object>)whereMethod.Invoke(null, new object[] { dbsetInstance, PropertyGetLambda(entityType, propertyName, propertyValue, Id) });

            return result.Any();
        }

        private LambdaExpression PropertyGetLambda(Type parameterType, string propertyName, string value, Guid? Id)
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
    }
}
