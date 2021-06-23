﻿using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using rbkApiModules.Infrastructure.Models.Charts.ChartJs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
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

            Builder.Config.Interaction = new InteractionOptions();
            Builder.Config.Interaction.SetAxis(AsixInteract.XY);
            Builder.Config.Interaction.Intersect = false;
            Builder.Config.Interaction.SetIntersectMode(IntersectMode.Nearest);
        }

        public virtual TFactory OfType(ChartType type)
        {
            if (this is RadialChart && 
                (type == ChartType.Bar || 
                 type == ChartType.Bubble || 
                 type == ChartType.Line || 
                 type == ChartType.Radar || 
                 type == ChartType.StackedBar || 
                 type == ChartType.Mixed || 
                 type == ChartType.HorizontalBar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for radial charts");
            }

            if (this is LinearChart && 
                (type == ChartType.Bubble || 
                 type == ChartType.Doughnut || 
                 type == ChartType.Pie || 
                 type == ChartType.PolarArea || 
                 type == ChartType.Radar))
            {
                throw new ArgumentException($"The type {type.ToString()} is not allowed for linear charts");
            }

            if (type == ChartType.Mixed)
            {
                Builder.Type = "bar";
            }
            else if (type == ChartType.StackedBar)
            {
                Builder.Type = "bar";
            }
            else if (type == ChartType.HorizontalBar)
            {
                Builder.Type = "bar";
                Builder.Config.IndexAxis = "y";

                // TODO: Acho que a melhor é criar um novo tipo de ponto, onde o Y é string e o X double,
                // e fazer a troca antes de serializar se for esse tipo de gráfico
                throw new NotImplementedException("Stacked bar charts are not yet implemented");
            }
            else
            {
                Builder.Type = type.ToString().Substring(0, 1).ToLower() + type.ToString().Substring(1);
            }


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
                Intersect = false
            };
            Builder.Config.Plugins.Tooltip.SetMode(TooltipMode.Nearest);

            return new TooltipBuilder<TFactory, TChart>(this);
        }

        public LegendBuilder<TFactory, TChart> WithLegend()
        {
            Builder.Config.Plugins.Legend = new LegendOptions()
            {
                Display = true,
            };

            Builder.Config.Plugins.Legend.SetPosition(PositionType.Bottom);

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

        public TFactory Responsive()
        {
            Builder.Config.MaintainAspectRatio = false;
            Builder.Config.Responsive = true;
            return (TFactory)this;
        }

        public ExpandoObject Build(bool debug = false)
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

            var deserialized = JsonConvert.DeserializeObject<ExpandoObject>(serializedWithoutNulls);

            if (debug)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(serializedWithoutNulls), Formatting.Indented));
            }

            return deserialized;
        } 
    } 
}
