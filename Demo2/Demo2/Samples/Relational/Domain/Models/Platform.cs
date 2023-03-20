using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class Platform : BaseEntity
{
    public Platform()
    {

    }

    public Platform(string name, Un un)
    {
        Name = name;
        Un = un;
    }

    public virtual string Name { get; private set; }

    public virtual Guid UnId { get; private set; }
    public virtual Un Un { get; private set; }
}
