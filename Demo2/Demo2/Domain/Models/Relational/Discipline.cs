using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class Discipline: BaseEntity
{
    private readonly HashSet<ChangeRequestToDiscipline> _changeRequests;
    public Discipline()
    {

    }
    public Discipline(string name)
    {
        _changeRequests = new HashSet<ChangeRequestToDiscipline>();

        Name = name;
    }

    public virtual string Name  { get; set; }

    public virtual IEnumerable<ChangeRequestToDiscipline> ChangeRequests => _changeRequests.ToList();

}
