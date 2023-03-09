using GCAB.Models.Domain;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class State : BaseEntity
{
    private HashSet<ChangeRequest> _items;

    public State()
    {

    }
    public State(string name)
    {
        _items = new HashSet<ChangeRequest>();

        Name = name; 
    }

    public virtual string Name { get; private set; }
}
