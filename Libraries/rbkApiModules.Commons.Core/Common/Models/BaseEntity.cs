namespace rbkApiModules.Commons.Core;

// TODO: Renomear para BaseGeneralEntity?
public abstract class BaseEntity : IBaseEntity
{
    protected BaseEntity()
    {

    }

    public virtual Guid Id { get; protected set; }
}
