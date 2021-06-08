using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearDataset: BaseDataset
    {
        public LinearDataset(string id): base(id)
        {
            Data = new List<Point>();
        }

        public DatasetType? Type { get; set; }

        public bool? Fill { get; set; }

        public string BackgroundColor { get; set; }
        public string BorderCapStyle { get; set; }
        public string BorderColor { get; set; }
        public double? BorderWidth { get; set; }
        public double? BorderRadius { get; set; }

        public double? CategoryPercentage { get; set; }
        public double? BarPercentage { get; set; }
        public double? BarThickness { get; set; }
        public double? MaxBarThickness { get; set; }
        public double? MinBarLength { get; set; }

        public string HoverBackgroundColor { get; set; }
        public string HoverBorderColor { get; set; }
        public string HoverBorderWidth { get; set; }

        public string PointBackgroundColor { get; set; }
        public string PointBorderColor { get; set; }
        public string PointBorderWidth { get; set; }
        public double? PointRadius { get; set; }
        public double? PointRotation { get; set; }

        public string PointHoverBackgroundColor { get; set; }
        public string PointHoverBorderColor { get; set; }
        public string PointHoverBorderWidth { get; set; }
        public string PointHoverRadius { get; set; }

        public double? Tension { get; set; }
        public double? PointHitRadius { get; set; }
        public string xAxisID { get; set; }
        public string yAxisID { get; set; }
        public PointStyle? PointStyle { get; set; }

        public List<Point> Data { get; set; }
    }

    public enum IndexAxis
    {
        X,
        Y
    }
}
