using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace rbkApiModules.Utilities.EFCore
{
    public static class NullableDateTimeWithoutKindConverter
    {
        public static ValueConverter<DateTime?, DateTime?> GetConverter()
        {
            var converter = new ValueConverter<DateTime?, DateTime?>(
                v => ToDatabase(v),
                v => FromDatabase(v));

            return converter;
        }

        private static DateTime? ToDatabase(DateTime? date)
        {
            return date;
        }

        private static DateTime? FromDatabase(DateTime? date)
        {
            if (date.HasValue)
            {
                return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
            }
            else
            {
                return null;
            }
        }
    }
}
