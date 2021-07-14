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
            Label = id;
        }

        public string Type { get; internal set; }

        public bool? Fill { get; internal set; }

        public string BackgroundColor { get; internal set; }
        public string BorderCapStyle { get; internal set; }
        public string BorderColor { get; internal set; }
        public double? BorderWidth { get; internal set; }
        public double? BorderRadius { get; internal set; }

        public double? CategoryPercentage { get; internal set; }
        public double? BarPercentage { get; internal set; }
        public double? BarThickness { get; internal set; }
        public double? MaxBarThickness { get; internal set; }
        public double? MinBarLength { get; internal set; }

        public string HoverBackgroundColor { get; internal set; }
        public string HoverBorderColor { get; internal set; }
        public string HoverBorderWidth { get; internal set; }

        public string PointBackgroundColor { get; internal set; }
        public string PointBorderColor { get; internal set; }
        public string PointBorderWidth { get; internal set; }
        public double? PointRadius { get; internal set; }
        public double? PointRotation { get; internal set; }

        public string PointHoverBackgroundColor { get; internal set; }
        public string PointHoverBorderColor { get; internal set; }
        public string PointHoverBorderWidth { get; internal set; }
        public string PointHoverRadius { get; internal set; }

        public double? Tension { get; internal set; }
        public double? PointHitRadius { get; internal set; }
        public string xAxisID { get; internal set; }
        public string yAxisID { get; internal set; }
        public string PointStyle { get; internal set; }

        public List<Point> Data { get; internal set; }
        public bool? BorderSkipped { get; internal set; }

        public void SetPointStyle(PointStyle style)
        {
            PointStyle = style.ToString().Substring(0, 1).ToLower() + style.ToString().Substring(1);
        }

        public void SetDatasetType(DatasetType type)
        {
            Type = type.ToString().ToLower();
        }
    }

    public enum IndexAxis
    {
        X,
        Y
    }
}
