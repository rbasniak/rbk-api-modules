using rbkApiModules.Infrastructure.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace rbkApiModules.Utilities
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T type) where T : Enum
        {
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(type.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string description;
            if (valueAttributes.Length > 0)
            {
                description = ((DescriptionAttribute)valueAttributes[0]).Description;
            }
            else
            {
                description = type.ToString();
            }

            return description;
        }

        public static SimpleNamedEntity<int> ToSimpleNamedEntity<T>(this T enumValue) where T : Enum
        {
            object underlyingValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumValue.GetType()));
            var id = (int)underlyingValue;

            return new SimpleNamedEntity<int>(id, enumValue.GetDescription());
        }
    }
}
