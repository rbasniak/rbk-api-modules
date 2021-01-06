using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class ChartOptions
    {
        public ChartOptions()
        {
            Title = new TitleOptions();
            Legend = new LegendOptions();
            Scales = new AxisOptions();
        }

        public TitleOptions Title { get; set; }
        public LegendOptions Legend { get; set; }
        public AxisOptions Scales { get; set; }
    }

    public class LegendOptions
    {
        public LegendOptions()
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

    public class TitleOptions
    {
        public bool Display => !String.IsNullOrEmpty(Text);
        public string Text { get; set; }
    }

    //public class ScaleOptions
    //{
    //    public ScaleOptions()
    //    {
    //        Scales = new AxisOptions();
    //    }

    //    public AxisOptions Scales { get; set; } 
    //}

    public class AxisOptions
    {
        public AxisOptions()
        {
            XAxes = new List<XAxisOptions>();
            YAxes = new List<YAxisOptions>();

            XAxes.Add(new XAxisOptions());
            YAxes.Add(new YAxisOptions());
        }

        public List<XAxisOptions> XAxes { get; set; }
        public List<YAxisOptions> YAxes { get; set; }
    }

    public class XAxisOptions
    {
        public XAxisOptions()
        {
            Ticks = new AxisTickOptions();
            Display = true;
        }

        public bool Display { get; set; }
        public AxisTickOptions Ticks { get; set; }
    }

    public class YAxisOptions
    {
        public YAxisOptions()
        {
            Ticks = new AxisTickOptions();
            GridLines = new GridLineOptions();
            Display = true;
        }

        public bool Display { get; set; }
        public AxisTickOptions Ticks { get; set; }
        public GridLineOptions GridLines { get; set; }
    }

    public class GridLineOptions
    {
        public GridLineOptions()
        {
            Display = true;
        }
        public bool Display { get; set; }
    }

    public class AxisTickOptions
    {
        public AxisTickOptions()
        {
            MaxTicksLimit = 0;
            AutoSkip = true;
            MinRotation = 0;
            MaxRotation = 50;
        }

        public int? MaxTicksLimit { get; set; }
        public bool? AutoSkip { get; set; }
        public int? MaxRotation { get; set; }
        public int? MinRotation { get; set; }
    }
}
