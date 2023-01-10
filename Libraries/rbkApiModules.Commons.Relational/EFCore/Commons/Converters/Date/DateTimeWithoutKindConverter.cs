using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace rbkApiModules.Commons.Relational;

public class DateTimeWithoutKindConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeWithoutKindConverter() : base(Serialize, Deserialize, null)
    {
    }
            
    static Expression<Func<DateTime, DateTime>> Deserialize = date => DateTime.SpecifyKind(date, DateTimeKind.Utc);
    static Expression<Func<DateTime, DateTime>> Serialize = date => date;
}
