using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.EventSourcing;

public class Un : BaseEntity
{
    private HashSet<Platform> _platforms;

    public Un()
    {

    }
    public Un(string name, string description, string domain, string repository = null)
    {
        _platforms = new HashSet<Platform>();

        Name = name;
        Description = description;
        Domain = domain;
        Repository = repository;
    }

    public virtual string Name { get; private set; }

    public virtual string Description { get; private set; }

    public virtual string Domain { get; private set; }

    public virtual string Repository { get; private set; }

    public virtual IEnumerable<Platform> Platforms => _platforms?.ToList();
}
