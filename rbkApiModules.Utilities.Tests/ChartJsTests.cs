using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var chart = data1.CreateLinearChart() 
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Mixed)
                .Padding(0, 0, 5, 0)
                .WithTitle("Índice de Atendimento de Gestão de Mudanças")  
                    .Padding(32, 32)
                    .Font(16)
                    .Alignment(AlignmentType.Center)
                    .Color("cornflowerblue")
                    .Chart
                .WithTooltips()  
                    .Chart
                .WithLegend() 
                    .At(PositionType.Bottom)
                    .Align(AlignmentType.Start)
                    .UsePointStyles()
                    .Title("Legenda")
                        .Align(AlignmentType.Start)
                        .Padding(0, 20, 0, 0)
                        .Legend
                    .Chart
                .SetupDataset("concluded") 
                    .OfType(DatasetType.Line)
                    .Label("Mudanças Concluídas")
                    .Color("#BE5651")
                    .Thickness(3)
                    .PointStyle(PointStyle.Triangle)
                    .PointRadius(5.0)
                    .CustomAxis("y")
                    .Chart
                .SetupDataset("open")   
                    .OfType(DatasetType.Line)
                    .Label("Total de Mudanças")
                    .Color("#4E81BD")
                    .PointStyle(PointStyle.RectRot)
                    .PointRadius(5.0)
                    .Thickness(3)
                    .CustomAxis("y")
                    .Chart
                .SetupDataset("iagm")  
                    .OfType(DatasetType.Bar)
                    .Label("IAGM")
                    .Color("#9DB167")
                    .BarPercentage(0.5)
                    .Thickness(2)
                    .CustomAxis("y1")
                    .ValuesRounding(1)
                    .Chart
                .WithYAxis("y")  
                    .At(AxisPosition.Left)
                    .Title("Qtde de mudanças")
                        .Padding(0, 0, 0, 10)
                        .Builder
                    .Range(0, null)
                    // .Overflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)
                    .Chart
                .WithYAxis("y1") 
                    .At(AxisPosition.Right)
                    .HideGridlines()
                    .StepSize(25)
                    .Title("IAGM (%)")
                        .Builder
                    .Range(0, 100)
                    // .Overflow(AxisOverflowType.Relative, AxisOverflowDirection.Both, 5)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulateComboChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 02, 01), 45),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 03, 01), 55),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 04, 01), 60),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 06, 01), 60),

                new NeutralDatePoint("Dataset 1", new DateTime(2021, 01, 01), 25),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 02, 01), 10),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 03, 01), 15),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 04, 01), 50),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 05, 01), 75),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 06, 01), 45),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Mixed)
                .WithTitle("Chart.js Combined Line/Bar Chart")
                    .Chart
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Dataset 1")
                    .OfType(DatasetType.Line)
                    .Label("Dataset 1")
                    .Color("#f53794")
                    .Thickness(2)
                    .Chart
                .SetupDataset("Dataset 2")
                    .OfType(DatasetType.Bar)
                    .Label("Dataset 2")
                    .Color("#537bc4", "77")
                    .Thickness(2)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulateLineChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 01, 01), 25),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 02, 01), 10),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 03, 01), 15),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 04, 01), 50),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 05, 01), 75),
                new NeutralDatePoint("Dataset 1", new DateTime(2021, 06, 01), 45),

                new NeutralDatePoint("Dataset 2", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 02, 01), 45),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 03, 01), 55),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 04, 01), 60),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Dataset 2", new DateTime(2021, 06, 01), 60),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Line)
                .WithTitle("Line Chart")
                    .Chart
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Dataset 1")
                    .OfType(DatasetType.Line)
                    .Label("Dataset 1")
                    .Color("#FF0000", "77")
                    .Chart
                .SetupDataset("Dataset 2")
                    .OfType(DatasetType.Line)
                    .Label("Dataset 2")
                    .Color("#5500FF", "77")
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulateRoundedBarChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Data 1", new DateTime(2021, 01, 01), 5),
                new NeutralDatePoint("Data 1", new DateTime(2021, 02, 01), 10),
                new NeutralDatePoint("Data 1", new DateTime(2021, 03, 01), 15),
                new NeutralDatePoint("Data 1", new DateTime(2021, 04, 01), 50),
                new NeutralDatePoint("Data 1", new DateTime(2021, 05, 01), 75),
                new NeutralDatePoint("Data 1", new DateTime(2021, 06, 01), 45),

                new NeutralDatePoint("Data 2", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 02, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 03, 01), 10),
                new NeutralDatePoint("Data 2", new DateTime(2021, 04, 01), 60),
                new NeutralDatePoint("Data 2", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Data 2", new DateTime(2021, 06, 01), 60),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Bar)
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Data 1")
                    .Label("Data 1")
                    .Color("#FF0000", "77")
                    .Thickness(2)
                    .RoundedBorders(5)
                    .Chart
                .SetupDataset("Data 2")
                    .Label("Data 2")
                    .Color("#5500FF", "77")
                    .Thickness(2)
                    .RoundedBorders(5)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulateStackedBarChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Data 1", new DateTime(2021, 01, 01), 5),
                new NeutralDatePoint("Data 1", new DateTime(2021, 02, 01), 10),
                new NeutralDatePoint("Data 1", new DateTime(2021, 03, 01), 15),
                new NeutralDatePoint("Data 1", new DateTime(2021, 04, 01), 50),
                new NeutralDatePoint("Data 1", new DateTime(2021, 05, 01), 75),
                new NeutralDatePoint("Data 1", new DateTime(2021, 06, 01), 45),

                new NeutralDatePoint("Data 2", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 02, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 03, 01), 10),
                new NeutralDatePoint("Data 2", new DateTime(2021, 04, 01), 60),
                new NeutralDatePoint("Data 2", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Data 2", new DateTime(2021, 06, 01), 60),

                new NeutralDatePoint("Data 3", new DateTime(2021, 01, 01), 2),
                new NeutralDatePoint("Data 3", new DateTime(2021, 02, 01), 15),
                new NeutralDatePoint("Data 3", new DateTime(2021, 03, 01), 5),
                new NeutralDatePoint("Data 3", new DateTime(2021, 04, 01), 30),
                new NeutralDatePoint("Data 3", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Data 3", new DateTime(2021, 06, 01), 10),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.StackedBar)
                .WithTitle("Bar Chart - Stacked")
                    .Chart
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Data 1")
                    .Label("Dataset 1")
                    .Color("#FF0000")
                    .Chart
                .SetupDataset("Data 2")
                    .Label("Dataset 2")
                    .Color("#0000FF")
                    .Chart
                .SetupDataset("Data 3")
                    .Label("Dataset 3")
                    .Color("#00FF00")
                    .Chart
                .Build(true);
        }

        // [Fact]
        //public void SimulateHorizontalBarChart()
        //{
        //    var data1 = new List<NeutralDatePoint>
        //    {
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 01, 01), 5),
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 02, 01), 10),
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 03, 01), 15),
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 04, 01), 50),
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 05, 01), 75),
        //        new NeutralDatePoint("Data 1", new DateTime(2021, 06, 01), 45),

        //        new NeutralDatePoint("Data 2", new DateTime(2021, 01, 01), 7),
        //        new NeutralDatePoint("Data 2", new DateTime(2021, 02, 01), 7),
        //        new NeutralDatePoint("Data 2", new DateTime(2021, 03, 01), 10),
        //        new NeutralDatePoint("Data 2", new DateTime(2021, 04, 01), 60),
        //        new NeutralDatePoint("Data 2", new DateTime(2021, 05, 01), 30),
        //        new NeutralDatePoint("Data 2", new DateTime(2021, 06, 01), 60),
        //    };

        //    var chart = data1.CreateLinearChart(GroupingType.Monthly)
        //        .OfType(ChartType.HorizontalBar)
        //        .WithTooltips()
        //            .Chart
        //        .WithLegend()
        //            .At(PositionType.Right)
        //            .Chart
        //        .SetupDataset("Data 1")
        //            .Label("Dataset 1")
        //            .Color("#FF0000", "77")
        //            .Thickness(2)
        //            .Chart
        //        .SetupDataset("Data 2")
        //            .Label("Dataset 2")
        //            .Color("#0000CC", "77")
        //            .Thickness(2)
        //            .Chart
        //        .Build(true);
        //}

        [Fact]
        public void SimulateVerticalBarChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Data 1", new DateTime(2021, 01, 01), 5),
                new NeutralDatePoint("Data 1", new DateTime(2021, 02, 01), 10),
                new NeutralDatePoint("Data 1", new DateTime(2021, 03, 01), 15),
                new NeutralDatePoint("Data 1", new DateTime(2021, 04, 01), 50),
                new NeutralDatePoint("Data 1", new DateTime(2021, 05, 01), 75),
                new NeutralDatePoint("Data 1", new DateTime(2021, 06, 01), 45),

                new NeutralDatePoint("Data 2", new DateTime(2021, 01, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 02, 01), 7),
                new NeutralDatePoint("Data 2", new DateTime(2021, 03, 01), 10),
                new NeutralDatePoint("Data 2", new DateTime(2021, 04, 01), 60),
                new NeutralDatePoint("Data 2", new DateTime(2021, 05, 01), 30),
                new NeutralDatePoint("Data 2", new DateTime(2021, 06, 01), 60),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Bar)
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Data 1")
                    .Label("Dataset 1")
                    .Color("#FF0000", "77")
                    .Thickness(0)
                    .Chart
                .SetupDataset("Data 2")
                    .Label("Dataset 2")
                    .Color("#00FF00", "77")
                    .Thickness(0)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulateDemoBarChart()
        {
            var data1 = new List<NeutralDatePoint>
            {
                new NeutralDatePoint("Data 1", new DateTime(2021, 01, 01), 12),
                new NeutralDatePoint("Data 1", new DateTime(2021, 02, 01), 19),
                new NeutralDatePoint("Data 1", new DateTime(2021, 03, 01), 3),
                new NeutralDatePoint("Data 1", new DateTime(2021, 04, 01), 5),
                new NeutralDatePoint("Data 1", new DateTime(2021, 05, 01), 2),
                new NeutralDatePoint("Data 1", new DateTime(2021, 06, 01), 3),
            };

            var chart = data1.CreateLinearChart()
                .PreparaData(GroupingType.Monthly)
                    .SeriesFrom(x => x.SerieId)
                    .DateFrom(x => x.Date)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .OfType(ChartType.Bar)
                .WithTooltips()
                    .Chart
                .WithTitle("# of Votes")
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .SetupDataset("Data 1")
                    .Label("Dataset 1")
                    .Color("#FF0000", "33")
                    .Thickness(0)
                    .Chart
                .Build(true);
        }


        [Fact]
        public void SimulateDougnutChart()
        {
            var data1 = new List<NeutralCategoryPoint>
                {
                    new NeutralCategoryPoint("Red", 5),
                    new NeutralCategoryPoint("Orange", 15),
                    new NeutralCategoryPoint("Yellow", 5),
                    new NeutralCategoryPoint("Green", 25),
                    new NeutralCategoryPoint("Blue", 20),
                };

            var chart = data1.CreateRadialChart()
                .OfType(ChartType.Doughnut)
                .Colors(new[] { "#4dc9f6", "#f67019", "#f53794", "#537bc4", "#acc236" })
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulatePieChart()
        {
            var data1 = new List<NeutralCategoryPoint>
                {
                    new NeutralCategoryPoint("Red", 5),
                    new NeutralCategoryPoint("Orange", 15),
                    new NeutralCategoryPoint("Yellow", 5),
                    new NeutralCategoryPoint("Green", 25),
                    new NeutralCategoryPoint("Blue", 20),
                };

            var chart = data1.CreateRadialChart()
                .OfType(ChartType.Pie)
                .Colors(new[] { "#4dc9f6", "#f67019", "#f53794", "#537bc4", "#acc236" })
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void SimulatePolarAreaChart()
        {
            var data1 = new List<NeutralCategoryPoint>
                {
                    new NeutralCategoryPoint("Red", 5),
                    new NeutralCategoryPoint("Orange", 15),
                    new NeutralCategoryPoint("Yellow", 5),
                    new NeutralCategoryPoint("Green", 25),
                    new NeutralCategoryPoint("Blue", 20),
                };

            var chart = data1.CreateRadialChart()
                .OfType(ChartType.PolarArea)
                .Colors(new[] { "#4dc9f6", "#f67019", "#f53794", "#537bc4", "#acc236" }, "44")
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Top)
                    .Chart
                .Build(true);
        }

        [Fact]
        public void GenerateCollorPalleteDemoCode()
        {
            var data1 = new List<NeutralCategoryPoint>
            {
                new NeutralCategoryPoint("A", 1),
                new NeutralCategoryPoint("B", 1),
                new NeutralCategoryPoint("C", 1),
                new NeutralCategoryPoint("D", 1),
                new NeutralCategoryPoint("E", 1),
            };

            var results = new List<object>();

            var values = (ColorPallete[])Enum.GetValues(typeof(ColorPallete));
            foreach (var pallete in values)
            {
                var chart = data1.CreateRadialChart()
                    .OfType(ChartType.Doughnut)
                    .Theme("77", pallete)
                    .Build(false);

                results.Add(new 
                { 
                    Name = pallete.ToString() + " = " + (int)pallete,
                    Chart = chart
                });
            }

            Debug.WriteLine(JsonConvert.SerializeObject(results, new JsonSerializerSettings 
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        [Fact]
        public void Sprints()
        {
            var data = new List<SpeedSeries>
            {
                new SpeedSeries("done", "Sprint88", 50),
                new SpeedSeries("extra", "Sprint88", 5),

                new SpeedSeries("done", "Sprint89", 75),
                new SpeedSeries("extra", "Sprint89", 3),

                new SpeedSeries("done", "Sprint90", 63),
                new SpeedSeries("extra", "Sprint90", 9),

                new SpeedSeries("done", "Sprint91", 95),
                new SpeedSeries("extra", "Sprint91", 0)
            };

            var results = data.CreateLinearChart()
                .PreparaData()
                    .SeriesFrom(x => x.SerieId)
                    .CategoryFrom(x => x.SprintName)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .Responsive()
                .OfType(ChartType.Bar)
                .Theme(ColorPallete.Spring1)
                .WithLegend()
                    .Chart
                .Build();

            Debug.WriteLine(JsonConvert.SerializeObject(results, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        [Fact]
        public void Sprints2()
        {
            var data = new List<SpeedSeries>
            {
                new SpeedSeries("done", "Sprint88", 50),
                new SpeedSeries("extra", "Sprint88", 5),

                new SpeedSeries("done", "Sprint89", 75),
                new SpeedSeries("extra", "Sprint89", 3),

                new SpeedSeries("done", "Sprint90", 63),
                new SpeedSeries("extra", "Sprint90", 9),

                new SpeedSeries("done", "Sprint91", 95),
                new SpeedSeries("extra", "Sprint91", 0)
            };

            var results = data.CreateLinearChart()
                .PreparaData()
                    .SingleSerie()
                    .CategoryFrom(x => x.SprintName)
                    .ValueFrom(x => x.Sum(x => x.Value))
                    .Chart
                .Responsive()
                .OfType(ChartType.Bar)
                .Theme(ColorPallete.Spring1)
                .WithLegend()
                    .Chart
                .Build();

            Debug.WriteLine(JsonConvert.SerializeObject(results, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        [Fact]
        public void Maneuvers()
        {
            var data = new List<ChartData>
            {
                new ChartData("Concluidas", 53, new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }),
                new ChartData("Não executadas", 47, new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }),
            };

            var results = data.CreateRadialChart()
                .PreparaData()
                    .SeriesFrom(x => x.Label)
                    .AppendExtraData(x => x.Data)
                    .ValueFrom(x => x.First().Value)
                    .Chart
                .OfType(ChartType.Pie)
                .Theme(ColorPallete.Spring1)
                .WithTooltips()
                    .Chart
                .WithLegend()
                    .At(PositionType.Bottom)
                    .Chart
                .Build();

            Debug.WriteLine(JsonConvert.SerializeObject(results, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        private class SpeedSeries
        {
            public SpeedSeries(string serieId, string sprintName, double value)
            {
                SerieId = serieId;
                Value = value;
                SprintName = sprintName;
            }

            public string SprintName { get; set; }
            public double Value { get; set; }
            public string SerieId { get; set; }
        }



        //    [Fact]
        //    public void SimulateIDTAChart()
        //    {
        //        var data1 = new List<NeutralCategoryPoint>
        //        {
        //            new NeutralCategoryPoint("0-90", "P-74", 2),
        //            new NeutralCategoryPoint("0-90", "P-75", 5),
        //            new NeutralCategoryPoint("0-90", "P-76", 3),
        //            new NeutralCategoryPoint("0-90", "P-77", 4),

        //            new NeutralCategoryPoint("90-180", "P-74", 1),
        //            new NeutralCategoryPoint("90-180", "P-75", 2),
        //            new NeutralCategoryPoint("90-180", "P-76", 3),
        //            new NeutralCategoryPoint("90-180", "P-77", 0),

        //            new NeutralCategoryPoint("180+", "P-74", 1),
        //            new NeutralCategoryPoint("180+", "P-75", 0),
        //            new NeutralCategoryPoint("180+", "P-76", 3),
        //            new NeutralCategoryPoint("180+", "P-77", 2),
        //        };

        //        var chart = data1.CreateLinearChart()
        //            .Type(ChartType.StackedBar)
        //            .WithDataset("0-90")
        //                .Type(ChartType.Bar)
        //                .Label("0 a 90 dias")
        //                .Color("#add28f", "77")
        //                .BorderWidth(2)
        //            .WithDataset("90-180")
        //                .Type(ChartType.Bar)
        //                .Label("90 a 180 dias")
        //                .Color("#f9e59c", "77")
        //                .BorderWidth(2)
        //            .WithDataset("180+")
        //                .Type(ChartType.Bar)
        //                .Label("Mais de 180 dias")
        //                .Color("#FA3511", "77")
        //                .BorderWidth(2)
        //            .WithTitle("Índice de Documentação Técnica Atualizada")
        //                .Padding(32)
        //                .FontSize(16)
        //            .WithTooltips()
        //            .WithLegend()
        //                .At(PositionType.Bottom)
        //            .HideXAxisGridlines()
        //            .XAxisBarPercentage(0.5)
        //            .WithYAxis()
        //                .Label("Qtde de Mudanças")
        //            .Build(true);
        //    }

        //[Fact]
        //public void SimulatePieChart()
        //{
        //    var data1 = new List<NeutralCategoryPoint>
        //        {
        //            new NeutralCategoryPoint("Curitiba", 2),
        //            new NeutralCategoryPoint("Curitiba", 2),
        //            new NeutralCategoryPoint("Curitiba", 2),
        //            new NeutralCategoryPoint("Curitiba", 2),
        //            new NeutralCategoryPoint("Curitiba", 2),

        //            new NeutralCategoryPoint("Rio de Janeiro", 2),
        //            new NeutralCategoryPoint("Rio de Janeiro", 2),

        //            new NeutralCategoryPoint("São Paulo", 2),
        //            new NeutralCategoryPoint("São Paulo", 2),
        //            new NeutralCategoryPoint("São Paulo", 2),
        //        };

        //    var chart = data1.CreateRadialChart()
        //        .OfType(ChartType.Pie)
        //        // .Theme(new[] { "#FF6384", "#36A2EB", "#FFCE56" })
        //        .WithTooltips()
        //            .Chart
        //        .WithLegend()
        //            .At(PositionType.Top)
        //            .Chart
        //        .Build(true);
        //}

        //    [Fact]
        //    public void SimulateDonutChart()
        //    {
        //        var data1 = new List<NeutralCategoryPoint>
        //        {
        //            new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba1", "Curitiba2" }),
        //            new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba3", "Curitiba4" }),
        //            new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba5", "Curitiba6" }),
        //            new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba7", "Curitiba8" }),
        //            new NeutralCategoryPoint("Curitiba", 2, new List<object> { "Curitiba9", "Curitiba10" }),

        //            new NeutralCategoryPoint("Rio de Janeiro", 2, new List<object> { "Rio de Janeiro 1", "Rio de Janeiro 2" }),
        //            new NeutralCategoryPoint("Rio de Janeiro", 2, new List<object> { "Rio de Janeiro 3", "Rio de Janeiro 4" }),

        //            new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 1", "São Paulo 2" }),
        //            new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 3", "São Paulo 4" }),
        //            new NeutralCategoryPoint("São Paulo", 2, new List<object> { "São Paulo 5", "São Paulo 6" }),

        //            new NeutralCategoryPoint("Florianópolis", 1, new List<object> { "Florianópolis 1" }),
        //        };

        //        var chart = data1.CreateRadialChart(3, "Outros")
        //            .OfType(ChartType.Doughnut)
        //            .WithColors(new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0" })
        //            .WithTooltips()
        //            .WithLegend()
        //                .At(PositionType.Right)
        //            .Build(true);
        //    }

        //    [Fact]
        //    public void SimulatePolarAreaChart()
        //    {
        //        var data1 = new List<NeutralCategoryPoint>
        //        {
        //            new NeutralCategoryPoint("Curitiba", 45),
        //            new NeutralCategoryPoint("Rio de Janeiro", 33),
        //            new NeutralCategoryPoint("São Paulo", 21),
        //            new NeutralCategoryPoint("Florianópolis", 9),
        //            new NeutralCategoryPoint("Porto Alegre", 5),
        //            new NeutralCategoryPoint("Manaus", 19),
        //        };

        //        var chart = data1.CreateRadialChart()
        //            .OfType(ChartType.PolarArea)
        //            .WithColors(ColorPallete.Pastel1, ColorPallete.Pastel2)
        //            .WithTooltips()
        //            .WithLegend()
        //                .At(PositionType.Right)
        //            .Build(true);
        //    }
    }

    public class ChartData
    {
        public ChartData(string label, double value, object data)
        {
            Label = label;
            Value = value;
            Data = data;
        }

        public string Label { get; set; }
        public double Value { get; set; }
        public object Data { get; set; }
    }
}

// TODO: Testar graficos do analytics 