namespace rbkApiModules.Utilities.Charts.ChartJs
{ 
    public class RadialDatasetBuilder<TFactory, TChart>: DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public RadialChartBuilder Builder => Chart as RadialChartBuilder;

        public RadialDatasetBuilder(BaseChartBuilder<TFactory, TChart> builder) : base(builder)
        {
        }
    } 
}