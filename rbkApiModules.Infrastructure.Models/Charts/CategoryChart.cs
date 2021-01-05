using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class CategoryChart
    {
        private int _colorIndex = 0;

        public CategoryChart(ColorPallete pallete)
        {
            Data = new CategoryChartData();
            Options = new ChartOptions();
            ColorPallete = pallete;
        }

        public CategoryChart(ColorPallete pallete, string title) : this(pallete)
        {
            Options.Title.Text = title;
        } 

        public CategoryChart(ColorPallete pallete, string id, string title): this(pallete, title)
        {
            Id = id;
        }

        public string Id { get; set; }
        public ColorPallete ColorPallete { get; set; }
        public CategoryChartData Data { get; set; }
        public ChartOptions Options { get; set; }

        public void AddData(string label, double value, string transparency = "ff", string color = null, double lineTension = 0)
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
        }
    }
}