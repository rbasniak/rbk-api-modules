using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public interface IEntityId
{
    new string ToString();
}

public abstract class EntityId : ValueObject, IEntityId
{
    public abstract override string ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ToString();
    }
}