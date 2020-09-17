using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Logging.Core
{
    public interface IRbkLogger
    {
        void WritePerformance(PerformanceData data);
        void WriteUsage(UsageData data);
        void WriteError(ErrorData data);
        void WriteDiagnostic(DiagnosticData data);
    }

    public class RbkLogger: IRbkLogger
    {
        private Logger _performanceLogger;
        private Logger _errorLogger;
        private Logger _usageLogger;
        private Logger _diagnosticLogger;

        public RbkLogger()
        {
            _performanceLogger = new LoggerConfiguration()
                .WriteTo.File(path: "D:\\performance.json")
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.File(path: "D:\\error.json")
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.File(path: "D:\\usage.json")
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: "D:\\diagnostic.json")
                .CreateLogger();
        }

        public void WritePerformance(PerformanceData data)
        {
            _performanceLogger.Write(LogEventLevel.Information, "{@PerformanceData}", data);
        }

        public void WriteUsage(UsageData data)
        {
            _usageLogger.Write(LogEventLevel.Information, "{@UsageData}", data);
        }

        public void WriteError(ErrorData data)
        {
            _errorLogger.Write(LogEventLevel.Information, "{@ErrorData}", data);
        }

        public void WriteDiagnostic(DiagnosticData data)
        {
            var writeDiagnostics = false; // Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);

            if (writeDiagnostics)
            {
                _diagnosticLogger.Write(LogEventLevel.Information, "{@DiagnosticData}", data);
            }
        }
    }

    public class PerformanceData : BaseLogData
    {

    }

    public class UsageData : BaseLogData
    {

    }

    public class ErrorData : BaseLogData
    {
        public string Exception { get; set; }
    }

    public class DiagnosticData: BaseLogData
    {

    }

    public class BaseLogData
    {
        public string Product { get; set; }
        public string Version { get; set; }
        public string Location { get; set; }
        public string Layer { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
}

