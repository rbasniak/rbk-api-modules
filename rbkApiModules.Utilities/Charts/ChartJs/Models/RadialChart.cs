namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialChart: BaseChart
    {
        public RadialChart(): base()
        {
            Data = new RadialChartData();
        }

        public RadialChartData Data { get; internal set; }
    }
}
