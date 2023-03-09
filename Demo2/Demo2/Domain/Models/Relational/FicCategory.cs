using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class FicCategory: BaseEntity
{
    private readonly HashSet<Fic> _fics;

    public FicCategory()
    {

    }
    public FicCategory(string name)
    {
        _fics = new HashSet<Fic>();

        Name = name;
    }

    public virtual string Name { get; set; }

    public virtual IEnumerable<Fic> Fics => _fics.ToList();
}
