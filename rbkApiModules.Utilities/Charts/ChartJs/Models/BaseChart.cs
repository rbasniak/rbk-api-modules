namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseChart
    {
        public BaseChart()
        {
            Config = new Configuration();

            Config.Plugins = new PluginOptions();
            Config.Plugins.Legend = new LegendOptions();
            Config.Plugins.Legend.Display = false;
        }

        public string Type { get; internal set; }
        public Configuration Config { get; internal set; }
        public bool AllowEmpty { get; internal set; }
    }
}
