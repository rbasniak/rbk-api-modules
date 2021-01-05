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
    }
}