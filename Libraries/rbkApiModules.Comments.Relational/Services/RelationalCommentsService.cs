using Microsoft.EntityFrameworkCore;
using rbkApiModules.Comments.Core;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Comments.Relational;

public class RelationalCommentsService : ICommentsService
{
    private readonly DbContext _context;
    private readonly IUserdataCommentService _userdataService;

    public RelationalCommentsService(IEnumerable<DbContext> contexts, IUserdataCommentService userdataService)
    {
        _context = contexts.GetDefaultContext();
        _userdataService = userdataService;
    }

    public async Task CreateAsync(string tenant, string username, string message, Guid entityId, Guid? parentId, CancellationToken cancellation = default)
    {
        var entity = new Comment(tenant, entityId, username, message, parentId);
        _context.Add(entity);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task<bool> ExistsAsync(string tenant, Guid id, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;
        return await _context.Set<Comment>().AnyAsync(x => x.Id == id && x.TenantId == tenant, cancellation);
    }

    public async Task<Comment[]> GetAllAsync(string tenant, Guid entityId, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var comments = await _context.Set<Comment>()
           .Include(x => x.Parent)
           .Where(x => x.EntityId == entityId && x.TenantId == tenant)
           .ToListAsync(cancellation);

        if (_userdataService != null)
        {
            await _userdataService.SetUserdata(comments);
        }

        var result = comments.Where(x => x.ParentId == null).OrderBy(x => x.Date).ToArray();

        return result;
    }
}