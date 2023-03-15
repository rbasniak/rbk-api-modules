using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class ChangeRequestType: BaseEntity
{
    public ChangeRequestType()
    {

    }
    public ChangeRequestType(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }
}
