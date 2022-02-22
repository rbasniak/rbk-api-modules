using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;

namespace rbkApiModules.Logs.Core
{
    public class GetDashboardData
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command() { }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public GroupingType GroupingType { get; set; }
        }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly ILogsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, ILogsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = new LogsDashboard();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo);

                results.TotalLogs = BuildTotalLogs(data, request.DateFrom, request.DateTo, request.GroupingType);

                results.SourceLogsLinear = BuildSourceLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.SourceLogsRadial = BuildSourceLogsRadial(data, request.DateFrom, request.DateTo);

                results.EnviromentLogsLinear = BuildEnviromentLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.EnviromentLogsRadial = BuildEnviromentLogsRadial(data, request.DateFrom, request.DateTo);

                results.AreaLogsLinear = BuildAreaLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.AreaLogsRadial = BuildAreaLogsRadial(data, request.DateFrom, request.DateTo);

                results.LayerLogsLinear = BuildLayerLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.LayerLogsRadial = BuildLayerLogsRadial(data, request.DateFrom, request.DateTo);

                results.UserLogsLinear = BuildUserLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.UserLogsRadial = BuildUserLogsRadial(data, request.DateFrom, request.DateTo);

                results.MachineLogsLinear = BuildMachineLogsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.MachineLogsRadial = BuildMachineLogsRadial(data, request.DateFrom, request.DateTo);

                return results;
            }

            private object BuildTotalLogs(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                       data.CreateLinearChart()
                           .PreparaData(groupingType)
                               .EnforceStartDate(from)
                               .EnforceEndDate(to)
                               .SingleSerie()
                               .DateFrom(x => x.Timestamp)
                               .ValueFrom(x => x.Count())
                               .Chart
                           .OfType(ChartType.Line)
                           .Theme(ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5, ColorPallete.Autumn1, ColorPallete.Autumn2)
                           .Responsive()
                           .WithTitle("Total logs")
                              .Font(16)
                              .Padding(8, 24)
                              .Chart
                           .WithTooltips()
                              .Chart
                           .WithYAxis("x")
                              .AutoSkip(10)
                              .Chart
                           .WithYAxis("y")
                              .Range(0, null)
                              .Chart
                           .SetupDatasets()
                              .Thickness(3)
                              .Chart
                           .Build();

                return chart;
            }

            private object BuildSourceLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.Source)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5)
                             .Responsive()
                             .WithTitle("Logs by source")
                                .Font(16)
                                .Padding(8, 24)
                                .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                             .WithTooltips()
                                .Chart
                             .WithYAxis("x")
                                .AutoSkip(10)
                                .Chart
                             .WithYAxis("y")
                                .Range(0, null)
                                .Chart
                             .SetupDatasets()
                                .Thickness(3)
                                .Chart
                             .Build();

                return chart;
            }

            private object BuildSourceLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.Source)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildEnviromentLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.Enviroment)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5)
                             .Responsive()
                             .WithTitle("Logs by enviroment")
                                .Font(16)
                                .Padding(8, 24)
                                .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                             .WithTooltips()
                                .Chart
                             .WithYAxis("x")
                                .AutoSkip(10)
                                .Chart
                             .WithYAxis("y")
                                .Range(0, null)
                                .Chart
                             .SetupDatasets()
                                .Thickness(3)
                                .Chart
                             .Build();

                return chart;
            }

            private object BuildEnviromentLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.Enviroment)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildAreaLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                        data.CreateLinearChart()
                            .PreparaData(groupingType)
                                .EnforceStartDate(from)
                                .EnforceEndDate(to)
                                .SeriesFrom(x => x.ApplicationArea)
                                .DateFrom(x => x.Timestamp)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Line)
                            .Theme(ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5, ColorPallete.Autumn1)
                            .Responsive()
                            .WithTitle("Logs by application area")
                               .Font(16)
                               .Padding(8, 24)
                               .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                            .WithTooltips()
                               .Chart
                            .WithYAxis("x")
                               .AutoSkip(10)
                               .Chart
                            .WithYAxis("y")
                               .Range(0, null)
                               .Chart
                            .SetupDatasets()
                               .Thickness(3)
                               .Chart
                            .Build();

                return chart;
            }

            private object BuildAreaLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ApplicationArea)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn2, ColorPallete.Autumn3)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildLayerLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                        data.CreateLinearChart()
                            .PreparaData(groupingType)
                                .EnforceStartDate(from)
                                .EnforceEndDate(to)
                                .SeriesFrom(x => x.ApplicationLayer)
                                .DateFrom(x => x.Timestamp)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Line)
                            .Theme(ColorPallete.Autumn4, ColorPallete.Autumn5, ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3)
                            .Responsive()
                            .WithTitle("Logs by application layer")
                               .Font(16)
                               .Padding(8, 24)
                               .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                            .WithTooltips()
                               .Chart
                            .WithYAxis("x")
                               .AutoSkip(10)
                               .Chart
                            .WithYAxis("y")
                               .Range(0, null)
                               .Chart
                            .SetupDatasets()
                               .Thickness(3)
                               .Chart
                            .Build();

                return chart;
            }

            private object BuildLayerLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ApplicationLayer)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn4, ColorPallete.Autumn5)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildUserLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                        data.CreateLinearChart()
                            .PreparaData(groupingType)
                                .EnforceStartDate(from)
                                .EnforceEndDate(to)
                                .SeriesFrom(x => x.Username)
                                .DateFrom(x => x.Timestamp)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Line)
                            .Theme(ColorPallete.Autumn4, ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3)
                            .Responsive()
                            .WithTitle("Logs by user")
                               .Font(16)
                               .Padding(8, 24)
                               .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                            .WithTooltips()
                               .Chart
                            .WithYAxis("x")
                               .AutoSkip(10)
                               .Chart
                            .WithYAxis("y")
                               .Range(0, null)
                               .Chart
                            .SetupDatasets()
                               .Thickness(3)
                               .Chart
                            .Build();

                return chart;
            }

            private object BuildUserLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.Username)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn4, ColorPallete.Autumn1)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildMachineLogsLinear(List<LogEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.MachineName)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4)
                             .Responsive()
                             .WithTitle("Logs by device")
                                .Font(16)
                                .Padding(8, 24)
                                .Chart
                             .WithLegend()
                                .At(PositionType.Bottom)
                                .Align(AlignmentType.Start)
                                .Chart
                             .WithTooltips()
                                .Chart
                             .WithYAxis("x")
                                .AutoSkip(10)
                                .Chart
                             .WithYAxis("y")
                                .Range(0, null)
                                .Chart
                             .SetupDatasets()
                                .Thickness(3)
                                .Chart
                             .Build();

                return chart;
            }

            private object BuildMachineLogsRadial(List<LogEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.MachineName)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }
        }
    }
}