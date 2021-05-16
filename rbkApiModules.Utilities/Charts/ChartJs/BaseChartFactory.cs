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
    public abstract class BaseChartFactory<TFactory, TChart> where TChart: BaseChart where TFactory: BaseChartFactory<TFactory, TChart>
    {
        protected TChart _chart;

        protected BaseChartFactory(TChart chart)
        {
            _chart = chart;
        }

        public TFactory ShowTitle(string title)
        {
            _chart.Config.Title = new TitleOptions()
            {
                Display = true,
                Text = title
            };

            return (TFactory)this;
        }

        public TFactory ShowLegend(PositionType position)
        {
            _chart.Config.Legend = new LegendOptions()
            {
                Display = true,
                Position = position.ToString().ToLower()
            };

            return (TFactory)this;
        }

        public TFactory SetType(ChartType type)
        {
            if (this is RadialChart && (type == ChartType.Bar || type == ChartType.Bubble || type == ChartType.Line || type == ChartType.Radar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for radial charts");
            }

            if (this is LinearChart && (type == ChartType.Bubble || type == ChartType.Doughnut || type == ChartType.Pie || type == ChartType.PolarArea || type == ChartType.Radar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for linear charts");
            }

            _chart.Type = type.ToString().Substring(0, 1).ToLower() + type.ToString().Substring(1);

            return (TFactory)this;
        }

        public TFactory SetTitlePadding(int value)
        {
            if (_chart.Config.Title == null) throw new NotSupportedException($"You need to call the '{nameof(ShowTitle)}' method before trying to setup the title");

            _chart.Config.Title.Padding = value;

            return (TFactory)this;
        }

        public TFactory SetTitleFontSize(int value)
        {
            if (_chart.Config.Title == null) throw new NotSupportedException($"You need to call the '{nameof(ShowTitle)}' method before trying to setup the title");

            _chart.Config.Title.FontSize = value;

            return (TFactory)this;
        }

        public TFactory EnableTooltips()
        {
            _chart.Config.Tooltips = new TooltipOptions()
            {
                Enabled = true,
                Mode = "index",
                Intersect = true
            };

            return (TFactory)this;
        }

        public object Build(bool debug = false)
        {
            if (_chart is LinearChart linearChart)
            {
                linearChart.Data.Labels = linearChart.Data.Datasets.First().Data.Select(x => x.X).ToList();
            }

            var serializedWithoutNulls = JsonConvert.SerializeObject(_chart, Formatting.Indented, new JsonSerializerSettings 
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
                var serialized2 = JsonConvert.SerializeObject(deserialized, Formatting.Indented);

                Debug.WriteLine(serialized2);
            }

            return null;
        }
    } 
}
