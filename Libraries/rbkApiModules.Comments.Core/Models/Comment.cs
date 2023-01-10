using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public class Comment : TenantEntity
{
    private BasicCommentInfo _userdata;
    private HashSet<Comment> _children;

    protected Comment()
    {

    }

    public Comment(string tenant, Guid entityId, string username, string message, Guid? parentId)
    {
        _children = new HashSet<Comment>();

        TenantId = tenant;
        EntityId = entityId;
        Username = username;
        ParentId = parentId;
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

    public virtual IEnumerable<Comment> Children => _children?.OrderBy(x => x.Date).ToList();

    public virtual void SetUserdata(BasicCommentInfo value)
    {
        _userdata = value;
    }
}

public class BasicCommentInfo
{
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required string Avatar { get; set; }
    public required DateTime Timestamp { get; set; }
}
