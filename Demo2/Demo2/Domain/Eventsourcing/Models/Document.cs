using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo2.EventSourcing;

public class Document 
{ 
    public Document()
    {

    }
    public Document(Guid id, string name, DocumentCategory category)
    {
        Id = id;
        Name = name;
        Category = category;
    }

    public virtual Guid Id { get; set; }
    public virtual string Name { get; set; }

    public virtual DocumentCategory Category { get; set; }
}


public enum DocumentCategory
{
    [Description("Category 1")]
    Category1 = 1,

    [Description("Category 2")]
    Category2 = 2,

    [Description("Category 3")]
    Category3 = 3,

    [Description("Category 4")]
    Category4 = 4,

    [Description("Category 5")]
    Category5 = 5,
}