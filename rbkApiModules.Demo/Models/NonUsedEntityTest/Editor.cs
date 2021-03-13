using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models
{
    public class Editor: BaseEntity
    {
        protected Editor()
        {

        }

        public Editor(string name, Blog blog)
        {
            Name = name;
            Blog = blog;
        }

        public string Name { get; private set; }

        public Guid BlogId { get; private set; }
        public Blog Blog { get; private set; }
    }
}
