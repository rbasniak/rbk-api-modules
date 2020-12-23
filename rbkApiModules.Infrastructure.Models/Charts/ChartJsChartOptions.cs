using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class ChartJsChartOptions
    {
        public ChartJsChartOptions()
        {
            Title = new ChartJsTitleOptions();
            Legend = new ChartJsLegendOptions();
            Scales = new ChartJsScaleOptions();
        }

        public ChartJsTitleOptions Title { get; set; }
        public ChartJsLegendOptions Legend { get; set; }
        public ChartJsScaleOptions Scales { get; set; }
    }

    public class ChartJsLegendOptions
    {
        public ChartJsLegendOptions()
        {
            Position = "bottom";
            FullWidth = true;
            Display = true;
            Align = "center";
        }

        public string Position { get; set; }
        public bool Display { get; set; }
        public string Align { get; set; }
        public bool FullWidth { get; set; }
    }

    public class ChartJsTitleOptions
    {
        public bool Display => !String.IsNullOrEmpty(Text);
        public string Text { get; set; }
    }

    public class ChartJsScaleOptions
    {
        public ChartJsScaleOptions()
        {
            Scales = new ChartJsAxisOptions();
        }

        public ChartJsAxisOptions Scales { get; set; }
    }

    public class ChartJsAxisOptions
    {
        public ChartJsAxisOptions()
        {
            XAxes = new ChartJsXAxisOptions();
            YAxes = new ChartJsYAxisOptions();
        }

        public ChartJsXAxisOptions XAxes { get; set; }
        public ChartJsYAxisOptions YAxes { get; set; }
    }

    public class ChartJsXAxisOptions
    {
        public ChartJsXAxisOptions()
        {
            Ticks = new ChartJsAxisTickOptions();
        }

        public bool Display { get; set; }
        public ChartJsAxisTickOptions Ticks { get; set; }
    }

    public class ChartJsYAxisOptions
    {
        public ChartJsYAxisOptions()
        {
            Ticks = new ChartJsAxisTickOptions();
            GridLines = new ChartJsGridLineOptions();
        }

        public bool Display { get; set; }
        public ChartJsAxisTickOptions Ticks { get; set; }
        public ChartJsGridLineOptions GridLines { get; set; }
    }

    public class ChartJsGridLineOptions
    {
        public ChartJsGridLineOptions()
        {
            Display = true;
        }
        public bool Display { get; set; }
    }

    public class ChartJsAxisTickOptions
    {
        public int MaxTicksLimit { get; set; }
    }
}
