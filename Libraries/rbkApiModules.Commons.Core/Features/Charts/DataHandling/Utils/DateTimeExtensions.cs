using System;

namespace rbkApiModules.Commons.Charts
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetGroupDate(this DateTime date, GroupingType groupingType)
        {
            switch (groupingType)
            {
                case GroupingType.None:
                    throw new NotSupportedException();
                case GroupingType.Hourly:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0).Date;
                case GroupingType.Daily:
                    return date.Date;
                case GroupingType.Weekly:
                    return date.StartOfWeek(DayOfWeek.Monday).Date;
                case GroupingType.Monthly:
                    return new DateTime(date.Year, date.Month, 1).Date;
                case GroupingType.Yearly:
                    return new DateTime(date.Year, 1, 1).Date;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
