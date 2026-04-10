namespace Demo1;

public class Plant : TenantEntity
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
