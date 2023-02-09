using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Tests.Integration;

internal abstract class EntityId
{
    private string _stringId;

    public EntityId(Guid id)
    {
        if (Prefix.Length != 4) throw new InvalidDataException("The prefix must contains 4 characters");

        Id = id;
        _stringId = $"{Prefix}/{Id.ToString().ToLower()}";
    }

    public Guid Id { get; protected set; }

    protected abstract string Prefix { get; }

    public override string ToString()
    {
        return _stringId;
    }
}
