using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Tests
{
    public static class MethodInfoHelper
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }
    } 
}
