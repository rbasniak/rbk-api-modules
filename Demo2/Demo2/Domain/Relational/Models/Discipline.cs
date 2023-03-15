using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class Discipline: BaseEntity
{
    public Discipline()
    {

    }
    public Discipline(string name)
    {
        Name = name;
    }

    public virtual string Name  { get; set; }

}
