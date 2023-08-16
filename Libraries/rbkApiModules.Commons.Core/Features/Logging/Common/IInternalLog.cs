using Serilog;

namespace rbkApiModules.Commons.Core.Logging;

public interface IAnalyticsLogger
{
    ILogger Logger { get; }
}

public class AnalyticsLogger :   IAnalyticsLogger
{
    private readonly ILogger _logger;
    public AnalyticsLogger(ILogger logger)
    {
        _logger = logger.ForContext("SourceContext", "Analytics");
    }

    public ILogger Logger => _logger;
}

public interface IDiagnosticsLogger
{
    ILogger Logger { get; }
}

public class DiagnosticsLogger : IDiagnosticsLogger
{
    private readonly ILogger _logger;
    public DiagnosticsLogger(ILogger logger)
    {
        _logger = logger.ForContext("SourceContext", "Diagnostics");
    }

    public ILogger Logger => _logger;
}
