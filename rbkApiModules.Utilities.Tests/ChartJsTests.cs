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
    public class ChartJsTests
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

            var chart = data1.CreateLinearChart(GroupingType.Monthly)
                .SetType(ChartType.Bar)
                .SetColors(new[] { "#BE5651", "#9DB167", "#4E81BD" })

                .SetCurrentDataset("concluded")
                    .SetDatasetType(ChartType.Line)
                    .SetDatasetLabel("Mudanças Concluídas")
                    .SetDatasetBorderWidth(3)
                    .SetDatasetYAxis("A")
                
                .SetCurrentDataset("open")
                    .SetDatasetType(ChartType.Line)
                    .SetDatasetLabel("Total de Mudanças")
                    .SetDatasetBorderWidth(3)
                    .SetDatasetYAxis("A")

                .SetCurrentDataset("iagm")
                    .SetDatasetType(ChartType.Bar)
                    .SetDatasetLabel("IAGM")
                    .SetDatasetBorderWidth(0)
                    .SetDatasetYAxis("B")
                    .SetDatasetValuesRounding(1)

                .SetDataSetOrder("concluded", "open", "iagm")

                .ShowTitle("Índice de Atendimento de Gestão de Mudanças")
                .SetTitlePadding(32)
                .SetTitleFontSize(16)

                .EnableTooltips()
                
                .ShowLegend(PositionType.Bottom)
                
                .HideXAxisGridlines()
                .SetXAxisBarPercentage(0.5)

                .AddYAxis("A")
                    .SetYAxisPosition(PositionType.Left)
                    .ShowYAxisLabel("Qtde de Mudanças")
                    .SetYAxisMinRange(0)
                    .SetYAxisOverflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)
                
                .AddYAxis("B")
                    .HideYAxisGridlines()
                    .SetYAxisPosition(PositionType.Right)
                    .ShowYAxisLabel("IAGM (%)")
                    .SetYAxisMinRange(0)
                    .SetYAxisMaxRange(100)
                    .SetYAxisOverflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)

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
                .SetType(ChartType.Bar)
                .UseStackedBars()
                .SetColors(new[] { "#add28f", "#f9e59c", "#FA3511" }, "77")

                .SetCurrentDataset("0-90")
                    .SetDatasetType(ChartType.Bar)
                    .SetDatasetLabel("0 a 90 dias")
                    .SetDatasetBorderWidth(2)

                .SetCurrentDataset("90-180")
                    .SetDatasetType(ChartType.Bar)
                    .SetDatasetLabel("90 a 180 dias")
                    .SetDatasetBorderWidth(2)

                .SetCurrentDataset("180+")
                    .SetDatasetType(ChartType.Bar)
                    .SetDatasetLabel("Mais de 180 dias")
                    .SetDatasetBorderWidth(2)

                .ShowTitle("Índice de Documentação Técnica Atualizada")
                .SetTitlePadding(32)
                .SetTitleFontSize(16)

                .EnableTooltips()

                .ShowLegend(PositionType.Bottom)

                .HideXAxisGridlines()
                .SetXAxisBarPercentage(0.5)

                .ShowYAxisLabel("Qtde de Mudanças")

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
                .SetType(ChartType.Pie)
                .SetColors(new[] { "#FF6384", "#36A2EB", "#FFCE56" })
                .ShowTitle("Título do Gráfico")
                .SetTitlePadding(32)
                .SetTitleFontSize(16)
                .EnableTooltips()
                .ShowLegend(PositionType.Right)
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
                .SetType(ChartType.Doughnut)
                .SetColors(new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" })
                .EnableTooltips()
                .ShowLegend(PositionType.Right)
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
                .SetType(ChartType.PolarArea)
                .SetColors(ColorPallete.Pastel1, ColorPallete.Pastel2)
                .EnableTooltips()
                .ShowLegend(PositionType.Right)
                .Build(true);
        }
    }
}

// TODO: Testar graficos do analytics 