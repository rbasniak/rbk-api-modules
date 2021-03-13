using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace rbkApiModules.Demo.Models
{
    public class Blog: BaseEntity
    {
        private HashSet<Editor> _editors;
        private HashSet<Post> _posts;

        protected Blog()
        {

        }

        public Blog(string name)
        {
            Name = name;

            _editors = new HashSet<Editor>();
            _posts = new HashSet<Post>();
        }

        public string Name { get; private set; }

        public IEnumerable<Editor> Editors => _editors?.ToList();
        public IEnumerable<Post> Posts => _posts?.ToList();
    }
}
