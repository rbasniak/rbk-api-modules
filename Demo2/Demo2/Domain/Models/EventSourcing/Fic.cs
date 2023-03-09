using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo2.EventSourcing;

public class Fic 
{
    public Fic()
    {

    }
    public Fic(Guid id, string name, FicCategory category)
    {
        Id = id;
        Name = name;
        Category = category;
    }

    public virtual Guid Id { get; set; }
    public virtual string Name { get; set; }
    public virtual FicCategory Category { get; set; }
}


public enum FicCategory
{
    [Description("FicCategory 1")]
    FicCategory1 = 1,

    [Description("FicCategory 2")]
    FicCategory2 = 2,

    [Description("FicCategory 3")]
    FicCategory3 = 3,

    [Description("FicCategory 4")]
    FicCategory4 = 4,

    [Description("FicCategory 5")]
    FicCategory5 = 5,

    [Description("FicCategory 6")]
    FicCategory6 = 6,

    [Description("FicCategory 7")]
    FicCategory7 = 7,
}