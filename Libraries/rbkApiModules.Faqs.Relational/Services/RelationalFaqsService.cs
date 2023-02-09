using Microsoft.EntityFrameworkCore;
using rbkApiModules.Faqs.Core;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Faqs.Relational;

public class RelationalFaqsService : IFaqsService
{
    private readonly DbContext _context;

    public RelationalFaqsService(IEnumerable<DbContext> contexts)
    {
        _context = contexts.GetDefaultContext();
    }

    public async Task<Faq[]> GetAllAsync(string tenant, string tag, CancellationToken cancellation = default)
    {
        var temp = await _context.Set<Faq>().ToListAsync();

        if (String.IsNullOrEmpty(tenant))
        {
            return await _context.Set<Faq>()
               .Where(x => x.Tag.ToLower() == tag.ToLower())
               .ToArrayAsync(cancellation);
        }
        else
        {
            return await _context.Set<Faq>()
               .Where(x => x.Tag.ToLower() == tag.ToLower() && (x.TenantId == null || x.TenantId == tenant))
               .ToArrayAsync(cancellation);
        }
    }

    public async Task<bool> ExistsAsync(string tenant, Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<Faq>()
           .AnyAsync(x => x.Id == id && x.TenantId == tenant, cancellation);
    }

    public async Task<Faq> CreateAsync(string tenant, string tag, string question, string answer, CancellationToken cancellation = default)
    {
        var entity = new Faq(tenant, tag.ToLower(), question, answer);

        _context.Add(entity);

        await _context.SaveChangesAsync(cancellation);

        return entity;
    }

    public async Task<Faq> UpdateAsync(string tenant, Guid id, string question, string answer, CancellationToken cancellation = default)
    {
        var faq = await _context.Set<Faq>().FindAsync(id);

        if (faq.TenantId != tenant)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        faq.Update(question, answer);
        
        await _context.SaveChangesAsync(cancellation);

        return faq;
    } 

    public async Task DeleteAsync(string tenant, Guid id, CancellationToken cancellation = default)
    {
        var faq = await _context.Set<Faq>().FindAsync(id);

        if (faq.TenantId != tenant)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        _context.Remove(faq);

        await _context.SaveChangesAsync(cancellation);
    }
}