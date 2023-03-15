using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.Relational;

public class Platform: BaseEntity
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
