using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace rbkApiModules.Commons.Relational;

public class NullableDateTimeWithoutKindConverter
    : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeWithoutKindConverter()
        : base(ToProvider, FromProvider, null)
    {
    }

    static readonly Expression<Func<DateTime?, DateTime?>> ToProvider =
        date => !date.HasValue
            ? (DateTime?)null
            : date.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
                : date.Value.Kind == DateTimeKind.Local
                    ? date.Value.ToUniversalTime()
                    : date.Value;

    static readonly Expression<Func<DateTime?, DateTime?>> FromProvider =
        date => !date.HasValue
            ? (DateTime?)null
            : DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
}
