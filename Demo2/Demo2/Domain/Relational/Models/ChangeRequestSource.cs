using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class ChangeRequestSource: BaseEntity
{
    public ChangeRequestSource()
    {

    }
    public ChangeRequestSource(string name)
    {
        Name = name;
    }

    public virtual string Name  { get; set; }

}
