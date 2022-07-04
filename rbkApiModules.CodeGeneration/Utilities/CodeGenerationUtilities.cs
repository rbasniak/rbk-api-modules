using rbkApiModules.Infrastructure.Models;
using System;
using System.Linq;
using System.Text;

namespace rbkApiModules.CodeGeneration
{
    public static class CodeGenerationUtilities
    {
        public static bool IsNative(Type type)
        {
            return type == typeof(string) ||
                type == typeof(Guid) ||
                type == typeof(Boolean) ||
                type == typeof(DateTime) ||
                type == typeof(Single) ||
                type == typeof(Double) ||
                type == typeof(Decimal) ||
                type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(Object) ||
                type.Name == typeof(SimpleNamedEntity<>).Name;
        }

        public static string ToTypeScriptFileCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            var newText = new StringBuilder(name.Length * 2);

            newText.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                    if ((name[i - 1] != ' ' && !char.IsUpper(name[i - 1])) ||
                        (false && char.IsUpper(name[i - 1]) &&
                         i < name.Length - 1 && !char.IsUpper(name[i + 1])))
                        newText.Append('-');
                newText.Append(name[i]);
            }
            return newText.ToString().ToLower();
        }

        public static string ToCamelCase(string name)
        {
            if (String.IsNullOrEmpty(name)) return name;

            return name.First().ToString().ToLower() + name.Substring(1);
        }
    }
}
