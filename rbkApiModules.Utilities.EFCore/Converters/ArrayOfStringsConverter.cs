using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.EFCore.Converters
{
    public static class ArrayOfStringsConverter
    {
        public static ValueConverter<string[], string> GetConverter(char separator)
        {
            var converter = new ValueConverter<string[], string>(
                v => ToDatabase(v, separator),
                v => FromDatabase(v, separator));

            return converter;
        }

        private static string ToDatabase(string[] array, char separator = ';')
        {
            if (array == null || array.Length == 0)
            {
                return String.Empty;
            }

            return String.Join(separator, array.OrderBy(x => x));
        }

        private static string[] FromDatabase(string value, char separator = ';')
        {
            if (String.IsNullOrEmpty(value))
            {
                return new string[] { };
            }

            return value.Split(separator);
        }
    }
}
