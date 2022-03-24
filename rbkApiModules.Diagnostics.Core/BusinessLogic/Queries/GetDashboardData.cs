using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities.Charts.ChartJs;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Diagnostics.Commons;

namespace rbkApiModules.Diagnostics.Core
{
    public class GetDashboardData
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public GroupingType GroupingType { get; set; }
        }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly IDiagnosticsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IDiagnosticsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = new DiagnosticsDashboard();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo);

                results.TotalErrors = BuildTotalErrors(data, request.DateFrom, request.DateTo, request.GroupingType);

                results.SourceErrorsLinear = BuildSourceErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.SourceErrorsRadial = BuildSourceErrorsRadial(data, request.DateFrom, request.DateTo);

                results.BrowserErrorsLinear = BuildBrowserErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.BrowserErrorsRadial = BuildBrowserErrorsRadial(data, request.DateFrom, request.DateTo);

                results.AreaErrorsLinear = BuildAreaErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.AreaErrorsRadial = BuildAreaErrorsRadial(data, request.DateFrom, request.DateTo);

                results.LayerErrorsLinear = BuildLayerErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.LayerErrorsRadial = BuildLayerErrorsRadial(data, request.DateFrom, request.DateTo);

                results.UserErrorsLinear = BuildUserErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.UserErrorsRadial = BuildUserErrorsRadial(data, request.DateFrom, request.DateTo);

                results.DeviceErrorsLinear = BuildDeviceErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.DeviceErrorsRadial = BuildDeviceErrorsRadial(data, request.DateFrom, request.DateTo);

                results.OperatingSystemErrorsLinear = BuildOperatingSystemErrorsLinear(data, request.DateFrom, request.DateTo, request.GroupingType);
                results.OperatingSystemErrorsRadial = BuildOperatingSystemErrorsRadial(data, request.DateFrom, request.DateTo);

                return results;
            }

            private object BuildBrowserErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.ClientBrowser.Split('.')[0])
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5)
                             .Responsive()
                             .WithTitle("Errors by browser")
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

            private object BuildBrowserErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ClientBrowser.Split('.')[0])
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


            private object BuildAreaErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
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
                            .WithTitle("Errors by application area")
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

            private object BuildAreaErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
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


            private object BuildTotalErrors(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
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
                           .WithTitle("Total errors")
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


            private object BuildLayerErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
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
                            .WithTitle("Errors by application layer")
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

            private object BuildLayerErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
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


            private object BuildDeviceErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.ClientDevice)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4)
                             .Responsive()
                             .WithTitle("Errors by device")
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

            private object BuildDeviceErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ClientDevice)
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


            private object BuildOperatingSystemErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.ClientOperatingSystem)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5)
                             .Responsive()
                             .WithTitle("Errors by operating system")
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

            private object BuildOperatingSystemErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ClientOperatingSystem)
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


            private object BuildSourceErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart =
                         data.CreateLinearChart()
                             .PreparaData(groupingType)
                                 .EnforceStartDate(from)
                                 .EnforceEndDate(to)
                                 .SeriesFrom(x => x.ExceptionSource)
                                 .DateFrom(x => x.Timestamp)
                                 .ValueFrom(x => x.Count())
                                 .Chart
                             .OfType(ChartType.Line)
                             .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2, ColorPallete.Autumn3, ColorPallete.Autumn4, ColorPallete.Autumn5)
                             .Responsive()
                             .WithTitle("Errors by exception source")
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

            private object BuildSourceErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
            {
                var chart = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .SeriesFrom(x => x.ExceptionSource)
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


            private object BuildUserErrorsLinear(List<DiagnosticsEntry> data, DateTime from, DateTime to, GroupingType groupingType)
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
                            .WithTitle("Errors by user")
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

            private object BuildUserErrorsRadial(List<DiagnosticsEntry> data, DateTime from, DateTime to)
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
        }
    }
}