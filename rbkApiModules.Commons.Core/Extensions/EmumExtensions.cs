using System.ComponentModel;

namespace rbkApiModules.Commons.Core;


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
}