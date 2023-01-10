using rbkApiModules.Commons.Core;

namespace Demo1.Models.Domain;

public class Plant: TenantEntity
{
    protected Plant()
    {

    }

    public Plant(string tenant, string name, string desciption)
    {
        Name = name;
        TenantId = tenant;
        Desciption = desciption;
    }

    public virtual string Name { get; protected set; }

    public virtual string Desciption { get; protected set; }
}
