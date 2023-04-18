using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace rbkApiModules.Commons.Diagnostics;

public class DiagnosticsEntry : BaseEntity
{
    public const string LOG_DATA_AREA = "log-data-area";

    public DiagnosticsEntry()
    {
        Timestamp = DateTime.UtcNow;
    }

    public DiagnosticsEntry(HttpContext context, string source, Exception exception, object input) : this()
    {
        var area = context.Items.FirstOrDefault(x => x.Key.ToString() == LOG_DATA_AREA);

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
        DatabaseExceptions = hasSqlException ? JsonSerializer.Serialize((sqlExceptions as List<Exception>).Select(x => x.ToBetterString()), new JsonSerializerOptions { WriteIndented = true }) : null;
        InputData = JsonSerializer.Serialize(input, new JsonSerializerOptions { WriteIndented = true });
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

    public string StackTrace { get; set; }

    public string DatabaseExceptions { get; set; }

    public string InputData { get; set; }

    public string ExtraData { get; set; }
}