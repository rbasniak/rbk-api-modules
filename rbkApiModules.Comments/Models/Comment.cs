﻿using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Comments
{
    public class Comment : BaseEntity
    {
        private BasicCommentInfo _userdata;
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

        public virtual BasicCommentInfo Userdata => _userdata;

        public virtual IEnumerable<Comment> Children => _children?.ToList();

        public virtual void SetUserdata(BasicCommentInfo value)
        {
            _userdata = value;
        }
    }

    public class BasicCommentInfo
    {
        public BasicCommentInfo(string username, string displayName, string avatar, DateTime timestamp)
        {
            Timestamp = timestamp;
            Username = username;
            Avatar = avatar;
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime Timestamp { get; set; }
    }
}