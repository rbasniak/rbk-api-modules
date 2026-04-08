using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public sealed class ApiKeyUsageTracker : IApiKeyUsageTracker
{
    private readonly DbContext _context;

    public ApiKeyUsageTracker(IEnumerable<DbContext> contexts)
    {
        _context = contexts.GetDefaultContext();
    }

    public async Task RecordSuccessfulAuthenticationAsync(Guid apiKeyId, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _context.Set<ApiKeyUsageByDay>()
            .FirstOrDefaultAsync(x => x.ApiKeyId == apiKeyId && x.Date == today, cancellationToken);

        if (existing == null)
        {
            await _context.AddAsync(new ApiKeyUsageByDay(apiKeyId, today, 1), cancellationToken);
        }
        else
        {
            existing.Increment();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
