namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public BaseChartBuilder<TFactory, TChart> Chart { get; }

        public DatasetBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Chart = chartBuilder;
        }
    }
}