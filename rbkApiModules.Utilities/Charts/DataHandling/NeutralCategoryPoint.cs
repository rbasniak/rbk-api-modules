using System;
using System.Collections.Generic;

namespace rbkApiModules.Utilities.Charts
{
    public class NeutralCategoryPoint
    {
        public NeutralCategoryPoint(string serieId, string category, double value, List<object> data = null)
        {
            Category = category;
            Value = value;
            SerieId = serieId;

            Data = data == null ? new List<object>() : data;
        }

        public NeutralCategoryPoint(string category, double value, List<object> data = null)
        {
            Category = category;
            Value = value;
            SerieId = String.Empty;

            Data = data == null ? new List<object>() : data;
        }

        public string Category { get; set; }
        public double Value { get; set; }
        public string SerieId { get; set; }
        public List<object> Data { get; set; }
    }
}
