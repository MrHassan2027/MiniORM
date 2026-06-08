using System.Reflection;
using System.Text;
using MiniORM.Attributes;

namespace MiniORM;

public static class SchemaBuilder
{
    private static readonly Dictionary<Type, string> TypeMap = new()
    {
        [typeof(int)] = "INTEGER",
        [typeof(long)] = "INTEGER",
        [typeof(string)] = "TEXT",
        [typeof(float)] = "REAL",
        [typeof(double)] = "REAL",
        [typeof(bool)] = "INTEGER",
        [typeof(DateTime)] = "TEXT",
    };

    public static string BuildCreateTable(Type type)
    {
        var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
        var sb = new StringBuilder($"CREATE TABLE IF NOT EXISTS {tableName} (");
        var cols = new List<string>();

        foreach (var prop in type.GetProperties())
        {
            var colName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
            var sqlType = TypeMap.GetValueOrDefault(prop.PropertyType, "TEXT");
            var isPk = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;
            var notNull = prop.GetCustomAttribute<NotNullAttribute>() != null;

            var col = $"{colName} {sqlType}";
            if (isPk) col += " PRIMARY KEY AUTOINCREMENT";
            else if (notNull) col += " NOT NULL";
            cols.Add(col);
        }

        sb.Append(string.Join(", ", cols));
        sb.Append(')');
        return sb.ToString();
    }
}
