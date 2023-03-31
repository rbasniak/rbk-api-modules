using Serilog;
using Serilog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Core.Logging;

public interface IInternalLogger
{
    ILogger Logger { get; }
}

public enum LogGroup
{
    Analytics,
    Internal,
    Diagnostics,
    Application
}

public class InternalLogger : IInternalLogger
{
    private readonly ILogger _logger;

    public InternalLogger()
    {
        _logger = Log.Logger.ForContext("Group", LogGroup.Internal);
    }

    public ILogger Logger => _logger;
}
