using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Demo.Models
{
    public class Post: BaseEntity
    {
        protected Post()
        {

        }

        public Post(string title, Blog blog)
        {
            Title = title;
            Blog = blog;
        }

        public string Title { get; private set; }

        public Guid BlogId { get; private set; }
        public Blog Blog { get; private set; }
    }
}
