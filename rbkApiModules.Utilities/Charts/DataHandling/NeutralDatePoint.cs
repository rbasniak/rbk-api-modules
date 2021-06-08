using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts
{
    public class NeutralDatePoint
    {
        public NeutralDatePoint(string serieId, DateTime date, double value, List<object> data = null)
        {
            Date = date;
            Value = value;
            SerieId = serieId;

            Data = data == null ? new List<object>() : data;
        }

        public NeutralDatePoint(string serieId, DateTime date, double value, int timezoneOffsetHours, List<object> data = null)
        {
            Date = date.AddHours(timezoneOffsetHours);
            Value = value;
            SerieId = serieId;

            Data = data == null ? new List<object>() : data;
        }

        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string SerieId { get; set; }
        public List<object> Data { get; set; }
    }
}
