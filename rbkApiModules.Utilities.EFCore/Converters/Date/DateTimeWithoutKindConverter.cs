using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.EFCore
{
    public static class DateTimeWithoutKindConverter
    {
        public static ValueConverter<DateTime, DateTime> GetConverter()
        {
            var converter = new ValueConverter<DateTime, DateTime>(
                v => ToDatabase(v),
                v => FromDatabase(v));

            return converter;
        }

        private static DateTime ToDatabase(DateTime date)
        {
            return date;
        }

        private static DateTime FromDatabase(DateTime date)
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
    }
}
