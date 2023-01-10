using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace rbkApiModules.Commons.Relational;

public class NullableDateTimeWithoutKindConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeWithoutKindConverter() : base(Serialize, Deserialize, null)
    {
    }

    static Expression<Func<DateTime?, DateTime?>> Deserialize = date => date.HasValue ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc) : null;
    static Expression<Func<DateTime?, DateTime?>> Serialize = date => date;
}
