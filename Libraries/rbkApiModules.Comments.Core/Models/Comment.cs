using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public class Comment : TenantEntity
{
    private BasicCommentInfo _userdata;
    private HashSet<Comment> _children;

    protected Comment()
    {

    }

    public Comment(string tenant, Guid entityId, string username, string message, Guid? parentId, string metadata = null)
    {
        _children = new HashSet<Comment>();

        TenantId = tenant;
        EntityId = entityId;
        Username = username;
        ParentId = parentId;
        Message = message;

        Date = DateTime.UtcNow;
    }
    public Comment(string tenant, Guid id, Guid entityId, string username, string message, Guid? parentId, string metadata = null)
        :this(tenant, entityId, username, message, parentId, metadata)
    {
        Id = id;
    }

    public virtual string Username { get; private set; }

    public virtual Guid EntityId { get; private set; }

    public virtual Guid? ParentId { get; private set; }
    public virtual Comment Parent { get; private set; }

    public virtual string Message { get; private set; }

    public virtual DateTime Date { get; private set; }

    public virtual string Metadata{ get; private set; }

    public virtual BasicCommentInfo Userdata => _userdata;

    public virtual IEnumerable<Comment> Children => _children?.OrderBy(x => x.Date).ToList();

    public virtual void SetUserdata(BasicCommentInfo value)
    {
        _userdata = value;
    }

    public virtual void ForceDate(DateTime date)
    {
        Date = date;
    }
}

public class BasicCommentInfo
{
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required string Avatar { get; set; }
    public required DateTime Timestamp { get; set; }
}
