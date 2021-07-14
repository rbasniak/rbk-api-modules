using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearDatasetBuilder<TFactory, TChart> : DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        private LinearDataset[] _datasets;

        public LinearDatasetBuilder(BaseChartBuilder<TFactory, TChart> builder, LinearDataset[] datasets): base(builder)
        {
            _datasets = datasets;
        }


        public LinearDatasetBuilder<TFactory, TChart> OfType(DatasetType type)
        {
            foreach (var dataset in _datasets)
            {
                dataset.SetDatasetType(type);

                if (type == DatasetType.Line)
                {
                    dataset.Tension = 0;
                    dataset.Fill = false;
                }
                else if (type == DatasetType.Bar)
                {
                    dataset.Fill = true;
                }
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> ValuesRounding(int decimals)
        {
            foreach (var dataset in _datasets)
            {
                foreach (var item in dataset.Data)
                {
                    item.RoundValue(decimals);
                }
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Label(string label)
        {
            foreach (var dataset in _datasets)
            {
                dataset.Label = label;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Thickness(double thickness)
        {
            foreach (var dataset in _datasets)
            {
                dataset.BorderWidth = thickness;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> CustomAxis(string axisId)
        {
            foreach (var dataset in _datasets)
            {
                dataset.yAxisID = axisId;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Color(string color, string transparency = "ff")
        {
            foreach (var dataset in _datasets)
            {
                dataset.BackgroundColor = color + transparency;
                dataset.BorderColor = color;

                dataset.PointBackgroundColor = color;
                dataset.PointBorderColor = color;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> BarPercentage(double value)
        {
            foreach (var dataset in _datasets)
            {
                dataset.BarPercentage = value;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> PointStyle(PointStyle style)
        {
            foreach (var dataset in _datasets)
            {
                dataset.SetPointStyle(style);
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> PointRadius(double radius, double? hitRadius = null)
        {
            foreach (var dataset in _datasets)
            {
                dataset.PointRadius = radius;

                if (hitRadius != null)
                {
                    dataset.PointHitRadius = hitRadius.Value;
                }
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> RoundedBorders(double radius)
        {
            foreach (var dataset in _datasets)
            {
                dataset.BorderRadius = radius;
                dataset.BorderSkipped = false;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> RoundToNearestStorageUnit(bool showUnitsInChartTitle = false)
        {
            foreach (var dataset in _datasets)
            {
                var averageSize = dataset.Data.Average(x => x.Y);
                var unit = String.Empty;

                foreach (var item in dataset.Data)
                {
                    if (averageSize < 1024.0)
                    {
                        unit = "bytes";
                    }
                    else if (averageSize < 1048576.0)
                    {
                        unit = "kb";
                        item.Y = Math.Round(item.Y / 1024.0, 1);
                    }
                    else if (averageSize < 1073741824.0)
                    {
                        unit = "mb";
                        item.Y = Math.Round(item.Y / 1048576.0, 1);
                    }
                    else if (averageSize < 1099511627776.0)
                    {
                        unit = "gb";
                        item.Y = Math.Round(item.Y / 1073741824.0, 1);
                    }
                    else if (averageSize < 1125899906842624.0)
                    {
                        unit = "tb";
                        item.Y = Math.Round(item.Y / 1099511627776.0, 1);
                    }

                    if (!String.IsNullOrEmpty(dataset.Label) && !dataset.Label.EndsWith($" ({unit})")) dataset.Label += $" ({unit})";
                }

                if (Builder.Builder.Config.Plugins.Title != null &&
                    !String.IsNullOrEmpty(Builder.Builder.Config.Plugins.Title.Text) &&
                    showUnitsInChartTitle)
                {
                    Builder.Builder.Config.Plugins.Title.Text += $" ({unit})";
                }
            }

            return this;
        }
    } 
}