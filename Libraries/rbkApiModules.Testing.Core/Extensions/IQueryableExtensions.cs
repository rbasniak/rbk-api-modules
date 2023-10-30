using rbkApiModules.Commons.Core;
using Shouldly;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace rbkApiModules.Testing.Core;

public static class IQueryableExtensions
{
    public static IQueryable<TSource> FromTenant<TSource>(this IQueryable<TSource> source, AuthenticatedRequest request) where TSource : TenantEntity
    {
        return source.Where(x => x.TenantId == request.Identity.Tenant);
    }
}