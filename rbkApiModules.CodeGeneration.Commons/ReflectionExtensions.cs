using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration.Commons
{
    public static class ReflectionExtensions
    {
        public static bool IsList(this Type type)
        {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
                    && type != typeof(string)
                    && type != typeof(DateTime);
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var attribute = type.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(T));

            return attribute != null ? (T)attribute : null;
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetAttribute<T>() != null;
        }

        public static T GetAttribute<T>(this MethodInfo methodInfo) where T : Attribute
        {
            var attribute = methodInfo.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(T));

            return attribute != null ? (T)attribute : null;
        }

        public static bool HasAttribute<T>(this MethodInfo methodInfo) where T : Attribute
        {
            return methodInfo.GetAttribute<T>() != null;
        }

        public static T GetAttribute<T>(this System.Reflection.PropertyInfo propertyInfo) where T : Attribute
        {
            var attribute = propertyInfo.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(T));

            return attribute != null ? (T)attribute : null;
        }

        public static bool HasAttribute<T>(this System.Reflection.PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetAttribute<T>() != null;
        }

        public static bool HasDateProperty(this Type type) 
        {
            var realType = type;

            if (type.IsList())
            {
                realType = type.GetInterfaces().Single(x => x.Name == typeof(IEnumerable<>).Name).GenericTypeArguments.First();
            }

            if (!realType.IsAssignableTo(typeof(BaseDataTransferObject)))
            {
                if (!realType.Assembly.FullName.Contains("rbpApiModules") && !realType.Assembly.FullName.Contains("DataTransfer") && !realType.FullName.Contains("DataTransfer")) return false;
            }

            return realType.GetProperties().Any(x => x.PropertyType == typeof(DateTime) || x.PropertyType.HasDateProperty());
        }


    }
}
