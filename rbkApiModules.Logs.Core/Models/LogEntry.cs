using rbkApiModules.Infrastructure.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Logs.Core
{
    public class LogEntry : BaseEntity
    {
        public const string LOG_ENTRY_AREA = "log-entry-area";
        public LogEntry()
        {
            Timestamp = DateTime.UtcNow;
        }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        [MaxLength(64)]
        public string ApplicationLayer { get; set; }

        [MaxLength(128)]
        public string ApplicationArea { get; set; }

        [MaxLength(64)]
        public string ApplicationVersion { get; set; }

        [MaxLength(255)]
        public string Source { get; set; }

        public string InputData { get; set; }

        [MaxLength(255)]
        public string Enviroment { get; set; }

        [MaxLength(64)]
        public string EnviromentVersion { get; set; }

        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string Domain { get; set; }

        [MaxLength(255)]
        public string MachineName { get; set; }
    }

    public enum LogLevel
    {
        [Description("Warning")]
        Warning,
        [Description("Error")]
        Error,
        [Description("Debug")]
        Debug
    }
}
