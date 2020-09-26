using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace rbkApiModules.Diagnostics.Core
{
    public class DiagnosticsEntry: BaseEntity
    {
        public string Exception { get; set; }

        public string ExtraInfo { get; set; }

        [MaxLength(64)]
        public string ApplicationArea { get; set; }

        [MaxLength(32)]
        public string ApplicationVersion { get; set; }

        [MaxLength(64)]
        public string ApplicationLayer { get; set; }

        [MaxLength(512)]
        public string Message { get; set; }

        [MaxLength(256)]
        public string Username { get; set; }

        [MaxLength(64)]
        public string Domain { get; set; }

        [MaxLength(64)]
        public string Source { get; set; }

        [MaxLength(256)]
        public string RequestId { get; set; }

        [MaxLength(256)]
        public string Browser { get; set; }

        [MaxLength(512)]
        public string UserAgent { get; set; }

        [MaxLength(64)]
        public string OperatinSystem { get; set; }

        [MaxLength(32)]
        public string OperatinSystemVersion { get; set; }

        [MaxLength(256)]
        public string Device { get; set; } 

        public DateTime Timestamp { get; set; }
    }
}
