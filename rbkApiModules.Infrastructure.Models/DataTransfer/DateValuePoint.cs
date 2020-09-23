using System;

namespace rbkApiModules.Infrastructure.Models
{
    public class DateValuePoint
    {
        public DateValuePoint(DateTime date, double value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}