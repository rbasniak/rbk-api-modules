using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class Discipline : BaseEntity
{
    public Discipline()
    {

    }
    public Discipline(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }

}
