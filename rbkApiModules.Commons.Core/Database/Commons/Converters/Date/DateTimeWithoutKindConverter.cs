using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace rbkApiModules.Commons.Relational;

public class DateTimeWithoutKindConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeWithoutKindConverter()
        : base(ToProvider, FromProvider, null)
    {
    }

    // write to DB
    static readonly Expression<Func<DateTime, DateTime>> ToProvider =
        date =>
            date.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date, DateTimeKind.Utc)     // assume unspecified = UTC
                : date.Kind == DateTimeKind.Local
                    ? date.ToUniversalTime()                       // normalize local -> UTC
                    : date;                                        // already UTC

    // read from DB
    static readonly Expression<Func<DateTime, DateTime>> FromProvider =
        date => DateTime.SpecifyKind(date, DateTimeKind.Utc);
}
