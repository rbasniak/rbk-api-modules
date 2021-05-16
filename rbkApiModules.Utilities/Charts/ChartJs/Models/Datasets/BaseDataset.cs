using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseDataset
    {
        public BaseDataset(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public double? BorderWidth { get; set; }
        public double? HoverBorderWidth { get; set; }
        public string BorderCapStyle { get; set; }
        public bool? Fill { get; set; }
        public double? LineTension { get; set; }

        public string PointBorderColor { get; set; }
        public string PointBackgroundColor { get; set; }
        public string PointBorderWidth { get; set; }
        public string PointRadius { get; set; }
        public string PointHoverRadius { get; set; }
        public string PointHitRadius { get; set; }
        public string PointHoverBackgroundColor { get; set; }
        public string PointHoverBorderColor { get; set; }
        public string PointHoverBorderWidth { get; set; }
        public PointStyle? PointStyle { get; set; }
        public string xAxisID { get; set; }
        public string yAxisID { get; set; }
        public string Type { get; set; }
        public bool? SpanGaps { get; set; }
        public bool? Hidden { get; set; }
        public bool? HideInLegendAndTooltip { get; set; }
        public bool? ShowLine { get; set; }
        public string Stack { get; set; }
    } 
}
