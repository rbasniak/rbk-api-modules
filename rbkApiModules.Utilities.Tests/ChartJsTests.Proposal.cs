using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace rbkApiModules.Utilities.Tests
{
    public class ChartJsTestsProposal
    {
        [Fact]
        public void SimulateIAGMChart()
        {
            var data1 = new List<NeutralDatePoint> 
            {
                new NeutralDatePoint("concluded", new DateTime(2021, 01, 01), 2),
                new NeutralDatePoint("concluded", new DateTime(2021, 02, 01), 3),
                new NeutralDatePoint("concluded", new DateTime(2021, 03, 01), 3),
                new NeutralDatePoint("concluded", new DateTime(2021, 04, 01), 4),
                new NeutralDatePoint("concluded", new DateTime(2021, 05, 01), 5),
                new NeutralDatePoint("concluded", new DateTime(2021, 06, 01), 5),

                new NeutralDatePoint("iagm", new DateTime(2021, 01, 01), 2.0 / 7.0 * 100.0),
                new NeutralDatePoint("iagm", new DateTime(2021, 02, 01), 3.0 / 11.0 * 100.0),
                new NeutralDatePoint("iagm", new DateTime(2021, 03, 01), 3.0 / 14.0 * 100.0),
                new NeutralDatePoint("iagm", new DateTime(2021, 04, 01), 4.0 / 15.0 * 100.0),
                new NeutralDatePoint("iagm", new DateTime(2021, 05, 01), 5.0 / 15.0 * 100.0),
                new NeutralDatePoint("iagm", new DateTime(2021, 06, 01), 5.0 / 27.0 * 100.0),

                new NeutralDatePoint("open", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("open", new DateTime(2021, 02, 01), 11),
                new NeutralDatePoint("open", new DateTime(2021, 03, 01), 14),
                new NeutralDatePoint("open", new DateTime(2021, 04, 01), 15),
                new NeutralDatePoint("open", new DateTime(2021, 05, 01), 15),
                new NeutralDatePoint("open", new DateTime(2021, 06, 01), 27),
            };

            var chart = data1.CreateLinearChart(GroupingType.Monthly) // Linear chart builder
                .OfType(ChartType.Bar)
                .HideXAxisGridlines()
                // .XAxisBarPercentage(0.5)
                .WithTitle("Índice de Atendimento de Gestão de Mudanças") // Title builder
                    .Padding(32, 32)
                    .Font(16)
                    .Chart
                .WithTooltips() //Tooltip builder
                    .Chart
                .WithLegend() // Legend builder
                    .At(PositionType.Bottom)
                    .Chart
                .WithDataset("concluded") // Dataset builder
                    .Type(ChartType.Line)
                    .Label("Mudanças Concluídas")
                    .Color("#BE5651")
                    .BorderWidth(3)
                    .CustomAxis("A")
                    .Chart
                .WithDataset("open")  // Dataset builder
                    .Type(ChartType.Line)
                    .Label("Total de Mudanças")
                    .Color("#9DB167")
                    .BorderWidth(3)
                    .CustomAxis("A")
                    .Chart
                .WithDataset("iagm")  // Dataset builder
                    .Type(ChartType.Bar)
                    .Label("IAGM")
                    .Color("#4E81BD")
                    .BorderWidth(0)
                    .UseAxis("B")
                    .ValuesRouding(1)
                    .Chart
                .WithYAxis("A") // YAxis builder
                    .At(PositionType.Left)
                    .Label("Qtde de mudanças")
                    .BeginAt(0)
                    .Overflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)
                    .Chart
                .WithYAxis("B") // YAxis builder
                    .At(PositionType.Right)
                    .HideGridlines()
                    .Label("IAGM (%)")
                    .BeginAt(0)
                    .FinishAt(100)
                    .Overflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)
                    .Chart
                .Build(true);
                
        } 

        [Fact]
        public void SimulateIDTAChart()
        {
            var data1 = new List<NeutralCategoryPoint>
            {
                new NeutralCategoryPoint("0-90", "P-74", 2),
                new NeutralCategoryPoint("0-90", "P-75", 5),
                new NeutralCategoryPoint("0-90", "P-76", 3),
                new NeutralCategoryPoint("0-90", "P-77", 4),

                new NeutralCategoryPoint("90-180", "P-74", 1),
                new NeutralCategoryPoint("90-180", "P-75", 2),
                new NeutralCategoryPoint("90-180", "P-76", 3),
                new NeutralCategoryPoint("90-180", "P-77", 0),

                new NeutralCategoryPoint("180+", "P-74", 1),
                new NeutralCategoryPoint("180+", "P-75", 0),
                new NeutralCategoryPoint("180+", "P-76", 3),
                new NeutralCategoryPoint("180+", "P-77", 2),
            };

            var chart = data1.CreateLinearChart()
                .Type(ChartType.StackedBar)
                .WithDataset("0-90")
                    .Type(ChartType.Bar)
                    .Label("0 a 90 dias")
                    .Color("#add28f", "77")
                    .BorderWidth(2)
                .WithDataset("90-180")
                    .Type(ChartType.Bar)
                    .Label("90 a 180 dias")
                    .Color("#f9e59c", "77")
                    .BorderWidth(2)
                .WithDataset("180+")
                    .Type(ChartType.Bar)
                    .Label("Mais de 180 dias")
                    .Color("#FA3511", "77")
                    .BorderWidth(2)
                .WithTitle("Índice de Documentação Técnica Atualizada")
                    .Padding(32)
                    .FontSize(16)
                .WithTooltips()
                .WithLegend()
                    .At(PositionType.Bottom)
                .HideXAxisGridlines()
                .XAxisBarPercentage(0.5)
                .WithYAxis()
                    .Label("Qtde de Mudanças")
                .Build(true);
        }

        [Fact]
        public void SimulatePieChart()
        {
            var data1 = new List<NeutralCategoryPoint>
            {
                new NeutralCategoryPoint("Curitiba", 2),
                new NeutralCategoryPoint("Curitiba", 2),
                new NeutralCategoryPoint("Curitiba", 2),
                new NeutralCategoryPoint("Curitiba", 2),
                new NeutralCategoryPoint("Curitiba", 2),

                new NeutralCategoryPoint("Rio de Janeiro", 2),
                new NeutralCategoryPoint("Rio de Janeiro", 2),

                new NeutralCategoryPoint("São Paulo", 2),
                new NeutralCategoryPoint("São Paulo", 2),
                new NeutralCategoryPoint("São Paulo", 2),
            };

            var chart = data1.CreateRadialChart()
                .OfType(ChartType.Pie)
                .WithColors(new[] { "#FF6384", "#36A2EB", "#FFCE56" })
                .WithTitle("Título do Gráfico")
                    .Padding(32)
                    .FontSize(16)
                .WithTooltips()
                .WithLegend()
                    .At(PositionType.Right)
                .Build(true);
        }

        [Fact]
        public void SimulateDonutChart()
        {
            var data1 = new List<NeutralCategoryPoint>
            {
                new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba1", "Curitiba2" }),
                new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba3", "Curitiba4" }),
                new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba5", "Curitiba6" }),
                new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba7", "Curitiba8" }),
                new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba9", "Curitiba10" }),

                new NeutralCategoryPoint("Rio de Janeiro", 2, new List<object> { "Rio de Janeiro 1", "Rio de Janeiro 2" }),
                new NeutralCategoryPoint("Rio de Janeiro", 2, new List<object> { "Rio de Janeiro 3", "Rio de Janeiro 4" }),

                new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 1", "São Paulo 2" }),
                new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 3", "São Paulo 4" }),
                new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 5", "São Paulo 6" }),

                new NeutralCategoryPoint("Florianópolis", 1, new List<object> { "Florianópolis 1" }),
            };

            var chart = data1.CreateRadialChart(3, "Outros")
                .OfType(ChartType.Doughnut)
                .WithColors(new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" })
                .WithTooltips()
                .WithLegend()
                    .At(PositionType.Right)
                .Build(true);
        }

        [Fact]
        public void SimulatePolarAreaChart()
        {
            var data1 = new List<NeutralCategoryPoint>
            {
                new NeutralCategoryPoint("Curitiba", 45),
                new NeutralCategoryPoint("Rio de Janeiro", 33),
                new NeutralCategoryPoint("São Paulo", 21),
                new NeutralCategoryPoint("Florianópolis", 9),
                new NeutralCategoryPoint("Porto Alegre", 5),
                new NeutralCategoryPoint("Manaus", 19),
            };

            var chart = data1.CreateRadialChart()
                .OfType(ChartType.PolarArea)
                .WithColors(ColorPallete.Pastel1, ColorPallete.Pastel2)
                .WithTooltips()
                .WithLegend()
                    .At(PositionType.Right)
                .Build(true);
        }
    }
}

// TODO: Testar graficos do analytics 