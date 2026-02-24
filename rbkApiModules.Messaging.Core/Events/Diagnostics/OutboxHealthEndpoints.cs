using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;

namespace PaintingProjectsManagement.Api.Diagnostics
{
    public static class OutboxHealthEndpoints
    {
        public static IEndpointRouteBuilder MapOutboxHealth(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/health/outbox", async (DbContext db, CancellationToken ct) =>
            {
                var now = DateTime.UtcNow;

                var query = db.Set<DomainOutboxMessage>()
                    .Where(x => x.ProcessedUtc == null && (x.DoNotProcessBeforeUtc == null || x.DoNotProcessBeforeUtc <= now));

                var count = await query.CountAsync(ct);
                var oldestCreatedUtc = await query
                    .OrderBy(x => x.CreatedUtc)
                    .Select(x => (DateTime?)x.CreatedUtc)
                    .FirstOrDefaultAsync(ct);

                var ageSeconds = oldestCreatedUtc.HasValue
                    ? Math.Max(0, (int)(now - oldestCreatedUtc.Value).TotalSeconds)
                    : 0;

                return Results.Ok(new
                {
                    unprocessedCount = count,
                    oldestUnprocessedAgeSeconds = ageSeconds,
                    nowUtc = now
                });
            });

            return endpoints;
        }
    }
} 