using rbkApiModules.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Comments
{
    public class Comment : BaseEntity
    {
        private HashSet<Comment> _children;

        protected Comment()
        {

        }

        public Comment(Guid entityId, string username, Comment parent, string message)
        {
            _children = new HashSet<Comment>();

            EntityId = entityId;
            Username = username;
            Parent = parent;
            Message = message;

            Date = DateTime.UtcNow;
        }

        public virtual string Username { get; private set; }

        public virtual Guid EntityId { get; private set; }

        public virtual Guid? ParentId { get; private set; }
        public virtual Comment Parent { get; private set; }

        public virtual string Message { get; private set; }

        public virtual DateTime Date { get; private set; }

        public virtual IEnumerable<Comment> Children => _children?.ToList();
    }
}
