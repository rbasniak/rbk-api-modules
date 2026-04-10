namespace rbkApiModules.Identity.Core.DataTransfer.Tenants;

public class TenantDetails 
{
    public string Alias { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public object? Metadata { get; set; }
}