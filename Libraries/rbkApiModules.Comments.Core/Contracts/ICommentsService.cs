namespace rbkApiModules.Comments.Core;

public interface ICommentsService
{
    Task<Comment[]> GetAllAsync(string tenant, Guid entityId, CancellationToken cancellation = default);
    Task CreateAsync(string tenant, string username, string message, Guid entityId, Guid? parentId, CancellationToken cancellation = default);
    Task<bool> ExistsAsync(string tenant, Guid id, CancellationToken cancellation = default);
}
