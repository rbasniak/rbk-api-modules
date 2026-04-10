namespace rbkApiModules.Commons.Core.Features.ApplicationOptions;

public class ApplicationOption
{
    private ApplicationOption()
    {
        // EF Core constructor, don't remove it
    }   

    public ApplicationOption(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public ApplicationOption(string key, string? tenantId, string? username, string value)
    {
        Key = key;
        TenantId = tenantId;
        Username = string.IsNullOrWhiteSpace(username) ? null : username.ToLower();
        Value = value;
    }

    public Guid Id { get; private set; }

    public string Key { get; private set; } = string.Empty;

    // Optional scope identifiers for overrides
    public string? TenantId { get; private set; }
    public string? Username { get; private set; }

    public string Value { get; private set; } = string.Empty;
}
