namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TooltipBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public BaseChartBuilder<TFactory, TChart> Chart { get; }

        public TooltipBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Chart = chartBuilder;
        }

        public TooltipBuilder<TFactory, TChart> BackgroundColor(string color)
        {
            Chart.Builder.Config.Plugins.Tooltip.BackgroundColor = color;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> UseSeriesPointStyles()
        {
            Chart.Builder.Config.Plugins.Tooltip.UsePointStyle = true;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> CornerRadius(double radius)
        {
            Chart.Builder.Config.Plugins.Tooltip.CornerRadius = radius;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> AtPointLocation()
        {
            Chart.Builder.Config.Plugins.Tooltip.SetTooltipPosition(TooltipPosition.Nearest);

            return this;
        }

        public TooltipBuilder<TFactory, TChart> Border(string color, double width)
        {
            Chart.Builder.Config.Plugins.Tooltip.BorderColor = color;
            Chart.Builder.Config.Plugins.Tooltip.BorderWidth = width;

            return this;
        }
    }
}