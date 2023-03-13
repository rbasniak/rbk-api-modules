using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class DocumentCategory: BaseEntity
{
    private readonly HashSet<Document> _documents;
    public DocumentCategory()
    {

    }
    public DocumentCategory(string name)
    {
        _documents = new HashSet<Document>();

        Name = name;
    }

    public virtual string Name  { get; set; }

    public virtual IEnumerable<Document> Documents => _documents.ToList();
}
