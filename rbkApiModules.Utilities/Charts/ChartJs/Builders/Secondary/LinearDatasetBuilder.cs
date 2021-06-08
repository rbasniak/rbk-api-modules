using System.Collections.Generic;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearDatasetBuilder<TFactory, TChart> : DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public LinearChartBuilder Builder => Chart as LinearChartBuilder;
        private LinearDataset _dataset;

        public LinearDatasetBuilder(BaseChartBuilder<TFactory, TChart> builder, LinearDataset dataset): base(builder)
        {
            _dataset = dataset;
        }


        public LinearDatasetBuilder<TFactory, TChart> OfType(DatasetType type)
        {
            _dataset.Type = type;

            if (type == DatasetType.Line)
            {
                _dataset.Tension = 0;
                _dataset.Fill = false;
            }
            else if (type == DatasetType.Bar)
            {
                _dataset.Fill = true;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> ValuesRounding(int decimals)
        {
            foreach (var item in _dataset.Data)
            {
                item.RoundValue(decimals);
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Label(string label)
        {
            _dataset.Label = label;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Thickness(double thickness)
        {
            _dataset.BorderWidth = thickness;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> CustomAxis(string axisId)
        {
            _dataset.yAxisID = axisId;

            return this;
        }
    } 
}