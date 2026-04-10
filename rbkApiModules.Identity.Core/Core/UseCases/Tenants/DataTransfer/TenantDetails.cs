namespace rbkApiModules.Identity.Core;

public class TenantDetails 
{
    public string Alias { get; set; } = string.Empty;   
    public string Name { get; set; } = string.Empty;
    public object Metadata { get; set; } = new();

    public static TenantDetails FromModel(Tenant tenant)
    {
        return new TenantDetails
        {
            Alias = tenant.Alias,
            Name = tenant.Name,
            Metadata = tenant.Metadata,
        };
    }
}