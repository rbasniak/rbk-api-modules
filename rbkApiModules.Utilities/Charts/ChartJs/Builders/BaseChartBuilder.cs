using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using rbkApiModules.Infrastructure.Models.Charts.ChartJs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseChartBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public TChart Builder { get; }

        protected BaseChartBuilder(TChart chart)
        {
            Builder = chart;
        }

        public TFactory OfType(ChartType type)
        {
            if (this is RadialChart && (type == ChartType.Bar || type == ChartType.Bubble || type == ChartType.Line || type == ChartType.Radar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for radial charts");
            }

            if (this is LinearChart && (type == ChartType.Bubble || type == ChartType.Doughnut || type == ChartType.Pie || type == ChartType.PolarArea || type == ChartType.Radar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for linear charts");
            }

            Builder.Type = type.ToString().Substring(0, 1).ToLower() + type.ToString().Substring(1);

            return (TFactory)this;
        }

        public TitleBuilder<TFactory, TChart> WithTitle(string title)
        {
            Builder.Config.Plugins.Title = new TitleOptions()
            {
                Display = true,
                Text = title
            };

            return new TitleBuilder<TFactory, TChart>(this);
        }

        public TooltipBuilder<TFactory, TChart> WithTooltips()
        {
            Builder.Config.Plugins.Tooltip = new TooltipOptions()
            {
                Enabled = true,
                Mode = TooltipMode.Nearest,
                Intersect = false
            }; 

            return new TooltipBuilder<TFactory, TChart>(this);
        }

        public LegendBuilder<TFactory, TChart> WithLegend()
        {
            Builder.Config.Plugins.Legend = new LegendOptions()
            {
                Display = true,
                Position = PositionType.Bottom
            };

            return new LegendBuilder<TFactory, TChart>(this);
        }

        public TFactory Padding(double left, double right, double top, double bottom)
        {
            this.Builder.Config.Layout = new LayoutOptions 
            {
                Padding = new PaddingOptions 
                {
                    Bottom = bottom,
                    Top = top,
                    Left = left,
                    Right = right
                }
            }; 
            return (TFactory)this;
        }

        public TFactory AspectRatio(double ratio)
        {
            Builder.Config.AspectRatio = ratio;
            Builder.Config.MaintainAspectRatio = true;

            return (TFactory)this;
        }

        public object Build(bool debug = false)
        {
            if (Builder is LinearChart linearChart)
            {
                linearChart.Data.Labels = linearChart.Data.Datasets.First().Data.Select(x => x.X).ToList();
            }

            var serializedWithoutNulls = JsonConvert.SerializeObject(Builder, Formatting.Indented, new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            var deserialized = JsonConvert.DeserializeObject(serializedWithoutNulls);

            if (debug)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(deserialized, Formatting.Indented));
            }

            return deserialized;
        } 
    } 
}
