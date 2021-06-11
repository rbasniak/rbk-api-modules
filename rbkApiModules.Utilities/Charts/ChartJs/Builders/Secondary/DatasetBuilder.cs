namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public TFactory Chart => Builder as TFactory;

        internal BaseChartBuilder<TFactory, TChart> Builder { get; }

        public DatasetBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Builder = chartBuilder;
        }
    }
}