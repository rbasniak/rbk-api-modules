using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Demo2.EventSourcing;

public class ChangeRequestPriorityInfo
{
    public ChangeRequestPriorityInfo()
    {

    }
    public ChangeRequestPriorityInfo(ChangeRequestPriority priority, string name, string color)
    {
        Name = name;
        Color = color;
        Id = (int)priority;
    }

    public virtual int Id { get; set; }

    public virtual string Name { get; set; }

    public virtual int Order { get; set; }

    public virtual string Color { get; set; }
}

public enum ChangeRequestPriority
{
    [Description("Baixa")]
    Low,

    [Description("Média")]
    Medium,

    [Description("Alta")]
    High,

    [Description("Urgente")]
    Urgent
}
