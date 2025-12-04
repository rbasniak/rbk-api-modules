namespace rbkApiModules.Commons.Core;

public interface ITelemetryPropagationDataCarrier
{
    string? TraceId { get; }
    string? ParentSpanId { get; }
    int? TraceFlags { get; }
    string? TraceState { get; }
}