using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace rbkApiModules.Commons.Core;

public static class DatabaseMetadata
{
    public sealed record Names(string Table, string Schema, IDictionary<string, string> Columns);

    public static Names For<TEntity>(DbContext db)
    {
        var entity = db.Model.FindEntityType(typeof(TEntity))!;
        var schema = entity.GetSchema() ?? string.Empty;
        var table = entity.GetTableName() ?? entity.GetDefaultTableName();
        var store = StoreObjectIdentifier.Table(table!, schema);

        var sql = db.GetService<ISqlGenerationHelper>();

        var columns = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in entity.GetProperties())
        {
            var columnName = property.GetColumnName(store) ?? property.GetDefaultColumnName();

            columns[property.Name] = sql.DelimitIdentifier(columnName, schema: null);
        }

        var qualified = string.IsNullOrEmpty(schema)
            ? sql.DelimitIdentifier(table!)
            : sql.DelimitIdentifier(table!, schema);

        return new Names(qualified, schema, columns);
    }
}