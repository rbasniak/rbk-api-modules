using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo1.Models.Domain.Folders;

public class File: BaseEntity
{
    protected File()
    {
        
    }

    public File(Folder folder, string name, string description, string extension, int size)
    {
        Folder = folder;
        Name = name;
        Description = description;
        Extension = extension;
        Size = size;
    }

    public int Size { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Extension { get; private set; }
    public Guid FolderId { get; private set; }
    public Folder Folder { get; private set; }
}
