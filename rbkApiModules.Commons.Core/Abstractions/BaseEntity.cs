public abstract class BaseEntity : IBaseEntity
{
    protected BaseEntity()
    {

    }

    public virtual Guid Id { get; protected set; }
}

public interface IBaseEntity
{
    Guid Id { get; }
}