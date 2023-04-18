namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class LinearChartData: BaseChartData
    {
        public LinearChartData(): base()
        {
            Datasets = new List<LinearDataset>();
        }
        public List<LinearDataset> Datasets { get; internal set; }
    }
}
