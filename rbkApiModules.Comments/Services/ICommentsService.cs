using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Comments
{
    public interface ICommentsService
    {
        Task<Comment[]> GetComments(Guid entityId);
    }

    public class CommentsService : ICommentsService
    {
        private readonly DbContext _context;

        public CommentsService(DbContext context)
        {
            _context = context;
        }

        public async Task<Comment[]> GetComments(Guid entityId)
        {
            var comments = await _context.Set<Comment>()
               .Include(x => x.Parent)
               .Where(x => x.EntityId == entityId)
               .ToListAsync();

            var result = comments.Where(x => x.ParentId == null).ToArray();

            return result;
        }
    }
}
