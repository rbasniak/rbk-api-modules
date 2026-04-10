using System.Diagnostics;

namespace rbkApiModules.Commons.Core;

public static class TelemetryUtils
{
    public static bool TryBuildUpstreamActivityContext(ITelemetryPropagationDataCarrier carrier, out ActivityContext parent)
    {
        parent = default;

        if (string.IsNullOrWhiteSpace(carrier.TraceId) || string.IsNullOrWhiteSpace(carrier.ParentSpanId))
        {
            return false;
        }

        try
        {
            var traceId = ActivityTraceId.CreateFromString(carrier.TraceId.AsSpan());
            var spanId = ActivitySpanId.CreateFromString(carrier.ParentSpanId.AsSpan());

            parent = new ActivityContext(
                traceId,
                spanId,
                (ActivityTraceFlags)(carrier.TraceFlags ?? 0),
                carrier.TraceState,
                isRemote: true);

            return true;
        }
        catch
        {
            parent = default;
            return false;
        }
    }
}
