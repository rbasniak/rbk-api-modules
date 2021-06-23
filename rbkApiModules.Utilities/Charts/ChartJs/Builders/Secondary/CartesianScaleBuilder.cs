using System;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class CartesianScaleBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public TFactory Chart => Builder as TFactory;
        internal BaseChartBuilder<TFactory, TChart> Builder { get; }
        private CartesianScale Scale { get; }

        public CartesianScaleBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder, CartesianScale scale)
        {
            Builder = chartBuilder;
            Scale = scale;
        }

        public CartesianScaleBuilder<TFactory, TChart> Stacked()
        {
            Scale.Stacked = true;
            return this;
        }

        public CartesianScaleBuilder<TFactory, TChart> HideGridlines()
        {
            if (Scale.Grid == null)
            {
                Scale.Grid = new GridLineOptions();
            }

            Scale.Grid.DrawOnChartArea = false;

            return this;
        } 

        public CartesianScaleBuilder<TFactory, TChart> At(AxisPosition position)
        {
            Scale.SetPosition(position);

            return this;
        }

        public CartesianTitleBuilder<TFactory, TChart> Title(string label)
        {
            Scale.Title = new ScaleTitleOptions
            {
                Display = true,
                Text = label,
            };

            return new CartesianTitleBuilder<TFactory, TChart>(this, Scale);
        }

        public CartesianScaleBuilder<TFactory, TChart> Range(double? min, double? max)
        {
            Scale.Min = min;
            Scale.Max = max;

            return this;
        }

        public CartesianScaleBuilder<TFactory, TChart> StepSize(int size)
        {
            if (Scale.Ticks == null)
            {
                Scale.Ticks = new TickOptions();
            }

            Scale.Ticks.StepSize = size;

            return this;
        }

        public CartesianScaleBuilder<TFactory, TChart> SetYAxisOverflow(AxisOverflowType type, AxisOverflowDirection direction, double value)
        {
            throw new NotImplementedException(); 
        }

        public CartesianScaleBuilder<TFactory, TChart> AutoSkip(int? padding = null)
        {
            if (Scale.Ticks == null)
            {
                Scale.Ticks = new TickOptions();
            }

            Scale.Ticks.AutoSkip = true;
            Scale.Ticks.AutoSkipPadding = padding;

            return this;
        }
    }

    public class CartesianTitleBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public CartesianScaleBuilder<TFactory, TChart> Builder { get; }
        private CartesianScale _scale;

        public CartesianTitleBuilder(CartesianScaleBuilder<TFactory, TChart> builder, CartesianScale scale)
        {
            Builder = builder;
            _scale = scale;
        }

        public CartesianTitleBuilder<TFactory, TChart> At(AxisPosition position)
        {
            _scale.SetPosition(position);

            return this;
        }

        public CartesianTitleBuilder<TFactory, TChart> Font(double size, double? lineHeight, string font)
        {
            _scale.Title.Font = new FontOptions
            {
                Family = font,
                LineHeight = lineHeight,
                Size = size
            };

            return this;
        }

        public CartesianTitleBuilder<TFactory, TChart> Padding(double left, double top, double right, double bottom)
        {
            _scale.Title.Padding = new PaddingOptions
            {
                Top = top,
                Left = left,
                Right = right,
                Bottom = bottom
            };

            return this;
        }
    }
}