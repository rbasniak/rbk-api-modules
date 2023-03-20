using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class Un : BaseEntity
{
    public Un()
    {

    }
    public Un(string name, string description, string domain, string repository = null)
    {
        Name = name;
        Description = description;
        Domain = domain;
        Repository = repository;
    }

    public virtual string Name { get; private set; }

    public virtual string Description { get; private set; }

    public virtual string Domain { get; private set; }

    public virtual string Repository { get; private set; }
}
