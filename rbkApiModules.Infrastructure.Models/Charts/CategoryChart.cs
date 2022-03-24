using System;
using System.Linq;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs.Deprecated
{
    public class CategoryChart: BaseChart
    {
        public CategoryChart(ColorPallete pallete): base(pallete)
        {
            Data = new CategoryChartData();
            Options = new ChartOptions();
            ColorPallete = pallete;
            Type = "bar";
        }

        public CategoryChart(ColorPallete pallete, string title) : base(pallete, title)
        {
            Data = new CategoryChartData();
            Options.Title.Text = title;
            Type = "bar";
        } 

        public CategoryChart(ColorPallete pallete, string id, string title): base(pallete, id, title)
        {
            Data = new CategoryChartData();
            Id = id;
            Type = "bar";
        }

        public CategoryChartData Data { get; set; }

        public void AddData(string label, double value, string transparency = "ff", string color = null)
        {
            var dataset = new CategoryDataSet();

            if (Data.Datasets.Count == 0)
            {
                Data.Datasets.Add(dataset);
            }
            else
            {
                dataset = Data.Datasets.First();
            }
            
            if (String.IsNullOrEmpty(color))
            {
                color = ChartCollorSelector.NextColor(ColorPallete, _colorIndex++);
            }

            Data.Labels.Add(label);
            dataset.Data.Add(value);
            dataset.BackgroundColor.Add(color + transparency);
            dataset.BorderColor.Add(color);
            dataset.BorderWidth = 2;
        }
    }
}