using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public abstract class BaseChart
    {
        protected int _colorIndex = 0;

        public BaseChart(ColorPallete pallete)
        {
            Options = new ChartOptions();
            ColorPallete = pallete;
        }

        public BaseChart(ColorPallete pallete, string title) : this(pallete)
        {
            Options.Title.Text = title;
        } 

        public BaseChart(ColorPallete pallete, string id, string title): this(pallete, title)
        {
            Id = id;
        }

        public string Id { get; set; }
        public ColorPallete ColorPallete { get; set; }
        public ChartOptions Options { get; set; }

        public void HideLegend()
        {
            Options.Legend.Display = false;
        }

        public void ShowLegend()
        {
            Options.Legend.Display = true;
        }

        public void SetTitle(string title)
        {
            Options.Title.Text = title;
        }

        public void SetMinX(double value)
        {
            Options.Scales.XAxes.First().Ticks.Min = value;
        }

        public void SetMaxX(double value)
        {
            Options.Scales.XAxes.First().Ticks.Max = value;
        }

        public void SetMinY(double value)
        {
            Options.Scales.YAxes.First().Ticks.Min = value;
        }

        public void SetMaxY(double value)
        {
            Options.Scales.YAxes.First().Ticks.Max = value;
        }

        public void UseDefaultCategoryAxes()
        {
            Options.Scales.XAxes.First().Ticks.AutoSkip = null;
            Options.Scales.XAxes.First().Ticks.MinRotation = null;
            Options.Scales.XAxes.First().Ticks.MaxRotation = null;
        }

        public void UseCompactCategoryAxes() 
        {
            Options.Scales.XAxes.First().Ticks.AutoSkip = false;
            Options.Scales.XAxes.First().Ticks.MinRotation = 90;
            Options.Scales.XAxes.First().Ticks.MaxRotation = 90;
        }


    }
}