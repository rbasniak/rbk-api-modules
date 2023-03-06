using AutoMapper;

namespace rbkApiModules.Identity.Core.DataTransfer.Tenants;

public class TenantDetails 
{
    public string Alias { get; set; }
    public string Name { get; set; }
    public object Metadata { get; set; }
}

public class TenantMappings : Profile
{
    public TenantMappings()
    {
        CreateMap<Tenant, TenantDetails>();
    }
}
