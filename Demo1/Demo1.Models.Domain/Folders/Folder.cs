using rbkApiModules.Commons.Core;

namespace Demo1.Models.Domain.Folders;

public class Folder: BaseEntity
{
    private HashSet<Folder> _children;
    private HashSet<File> _files;

    protected Folder()
    {
        
    }

    private Folder(Folder parent, string name, string description)
    {
        Parent = parent;
        Name = name;
        Description = description;

        _children = new HashSet<Folder>();
        _files = new HashSet<File>();
    }

    public Folder(string name, string description): this(null, name, description) 
    {
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public Folder Parent { get; private set; }
    public IEnumerable<Folder> Children => _children?.ToList();
    public IEnumerable<File> Files => _files?.ToList();

    public Folder AddChild(string name, string description)
    {
        var folder = new Folder(this, name, description);
        
        _children.Add(folder);
        
        return folder;
    }

    public void AddFile(string name, string description, string extension, int size)
    {
        _files.Add(new File(this, name, description, extension, size));
    }
}
