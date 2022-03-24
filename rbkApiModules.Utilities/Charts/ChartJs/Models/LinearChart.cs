namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearChart : BaseChart
    {
        public LinearChart() : base()
        {
            Data = new LinearChartData();
        }

        public LinearChartData Data { get; internal set; }
    }
}
