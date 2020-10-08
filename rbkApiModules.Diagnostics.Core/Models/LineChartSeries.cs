using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace rbkApiModules.Diagnostics.Core
{
    public class LineChartSeries
    {
        public LineChartSeries()
        {
            Data = new List<DateValuePoint>();
        }

        public LineChartSeries(string name)
        {
            Name = String.IsNullOrEmpty(name) ? "Unknown" : name;

            Data = new List<DateValuePoint>();
        }

        public string Name { get; set; }

        public List<DateValuePoint> Data { get; set; }
    }
}