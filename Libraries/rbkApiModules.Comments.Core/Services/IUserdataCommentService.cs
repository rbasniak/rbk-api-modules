namespace rbkApiModules.Comments.Core;

public interface IUserdataCommentService 
{
    Task SetUserdata(List<Comment> comments, CancellationToken cancellation = default);
}
