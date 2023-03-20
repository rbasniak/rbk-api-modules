using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class Document : BaseEntity
{
    public Document()
    {

    }
    public Document(string name, DocumentCategory category, ChangeRequest changeRequest)
    {
        Name = name;
        Category = category;
        ChangeRequest = changeRequest;
    }

    public virtual string Name { get; set; }

    public virtual string SindotecUrl { get; set; }

    public virtual Guid CategoryId { get; set; }
    public virtual DocumentCategory Category { get; set; }

    public virtual Guid ChangeRequestId { get; set; }
    public virtual ChangeRequest ChangeRequest { get; set; }
}
