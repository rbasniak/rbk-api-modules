namespace rbkApiModules.Commons.Core;

public interface IRequestContext
{
    string TenantId { get; internal set; }
    string Username { get; internal set; }
    string CorrelationId { get; internal set; }
    string CausationId { get; internal set; }
    bool IsAuthenticated { get; }
    bool HasTenant { get; }
}

public sealed class RequestContext : IRequestContext
{
    public string TenantId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string CausationId { get; set; } = string.Empty;

    public bool IsAuthenticated => string.IsNullOrEmpty(Username) == false;
    public bool HasTenant => string.IsNullOrEmpty(TenantId) == false;
}