namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class YAxisBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public BaseChartBuilder<TFactory, TChart> Chart { get; }

        public YAxisBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Chart = chartBuilder;
        }
    }
}