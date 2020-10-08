using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace rbkApiModules.Diagnostics.Commons
{
    public class DiagnosticsEntry: BaseEntity
    {
        public DiagnosticsEntry()
        {
            Timestamp = DateTime.UtcNow;
        } 

        public DiagnosticsEntry(HttpContext context, string source, Exception exception, object input): this()
        {
            var area = context.Items.FirstOrDefault(x => x.Key.ToString() == "log-data-area");

            var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
            var username = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var domain = user.Claims.FirstOrDefault(c => c.Type == "domain")?.Value;

            var hasSqlException = context.Items.TryGetValue("sql-exception", out object sqlExceptions);

            ApplicationArea = area.Key != null ? area.Value as string : String.Empty;
            ApplicationLayer = "API";
            ApplicationVersion = "1.0.0"; // TODO: pegar de algum lugar
            ClientBrowser = "";
            ClientDevice = "Server";
            Domain = domain;
            StackTrace = exception.ToBetterString();
            DatabaseExceptions = hasSqlException ? JsonConvert.SerializeObject((sqlExceptions as List<Exception>).Select(x => x.ToBetterString()), Formatting.Indented) : null;
            InputData = JsonConvert.SerializeObject(input, Formatting.Indented);
            ExceptionMessage = exception.Message;
            ClientOperatingSystem = Environment.OSVersion.Platform.ToString();
            ClientOperatingSystemVersion = Environment.OSVersion.VersionString;
            RequestId = "";
            ExceptionSource = source;
            ClientUserAgent = context.Request.Headers["User-Agent"];
            Username = username;
        }

        public DateTime Timestamp { get; set; }

        [MaxLength(128)]
        public string ApplicationArea { get; set; }

        [MaxLength(64)]
        public string ApplicationVersion { get; set; }

        [MaxLength(64)]
        public string ApplicationLayer { get; set; }

        [MaxLength(512)]
        public string ExceptionMessage { get; set; }

        [MaxLength(256)]
        public string Username { get; set; }

        [MaxLength(128)]
        public string Domain { get; set; }

        [MaxLength(256)]
        public string ExceptionSource { get; set; }

        [MaxLength(256)]
        public string RequestId { get; set; }

        [MaxLength(256)]
        public string ClientBrowser { get; set; }

        [MaxLength(512)]
        public string ClientUserAgent { get; set; }

        [MaxLength(255)]
        public string ClientOperatingSystem { get; set; }

        [MaxLength(255)]
        public string ClientOperatingSystemVersion { get; set; }

        [MaxLength(256)]
        public string ClientDevice { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string StackTrace { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string DatabaseExceptions { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string InputData { get; set; }
    }
}
