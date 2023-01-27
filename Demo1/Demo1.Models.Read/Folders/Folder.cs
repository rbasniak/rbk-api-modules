using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo1.Models.Read.Folders;

public class Folder
{
    public string Name { get; set; }
    public int Size
    {
        get
        {
            var childrenSize = 0;

            if (Children != null)
            {
                childrenSize = Children.Sum(x => x.Size);
            }

            var filesSize = 0;

            if (Files != null)
            {
                filesSize = Files.Sum(x => x.Size);
            }

            return childrenSize + filesSize;
        }
    }

    public Folder[] Children { get; set; }

    public File[] Files { get; set; }
}

public class File
{
    public string Name { get; set; }
    public int Size { get; set; }
}