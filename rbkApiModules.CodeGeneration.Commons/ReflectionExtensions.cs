﻿using System;
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

        public static T GetAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            var attribute = propertyInfo.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(T));

            return attribute != null ? (T)attribute : null;
        }

        public static bool HasAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetAttribute<T>() != null;
        }
    }
}
