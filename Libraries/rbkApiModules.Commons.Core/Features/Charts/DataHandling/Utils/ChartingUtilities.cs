using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities
{
    public static class ChartingUtilities
    {
        public static List<DateValuePoint> BuildLineChartAxis(DateTime startDate, DateTime endDate)
        {
            var date = startDate;

            var axis = new List<DateValuePoint>();

            while (date <= endDate.Date)
            {
                axis.Add(new DateValuePoint(date.Date, 0));
                date = date.AddDays(1);
            }

            if (axis.Last().Date != endDate.Date)
            {
                axis.Add(new DateValuePoint(endDate.Date, 0));
            }

            return axis;
        }
    }
}
