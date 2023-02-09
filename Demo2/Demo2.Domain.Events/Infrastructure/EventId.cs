using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public interface IEntityId
{
    string ToString();
} 

public abstract class EntityId : ValueObject, IEntityId
{
    private readonly Guid _id;

    public EntityId(Guid id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return _id.ToString().ToLower();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ToString();
    }
}