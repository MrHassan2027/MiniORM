using System.Data;
using System.Reflection;
using System.Text;
using MiniORM.Attributes;

namespace MiniORM;

public class Database
{
    private readonly Func<IDbConnection> _factory;

    public Database(Func<IDbConnection> connectionFactory) => _factory = connectionFactory;

    public void CreateTable<T>()
    {
        var sql = SchemaBuilder.BuildCreateTable(typeof(T));
        Execute(sql);
    }

    public void Insert<T>(T entity)
    {
        var type = typeof(T);
        var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
        var props = GetMappedProps(type);

        var cols = string.Join(", ", props.Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name));
        var vals = string.Join(", ", props.Select(p => $"@{p.Name}"));

        var conn = _factory();
        if (conn.State != ConnectionState.Open) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"INSERT INTO {tableName} ({cols}) VALUES ({vals})";
        foreach (var p in props)
            AddParam(cmd, $"@{p.Name}", p.GetValue(entity));
        cmd.ExecuteNonQuery();
    }

    public List<T> Query<T>(Func<T, bool>? predicate = null) where T : new()
    {
        var type = typeof(T);
        var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
        var props = type.GetProperties().ToList();
        var results = new List<T>();

        var conn = _factory();
        if (conn.State != ConnectionState.Open) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {tableName}";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var obj = new T();
            foreach (var p in props)
            {
                var colName = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
                try { p.SetValue(obj, Convert.ChangeType(reader[colName], p.PropertyType)); }
                catch { }
            }
            if (predicate == null || predicate(obj)) results.Add(obj);
        }
        return results;
    }

    private void Execute(string sql)
    {
        var conn = _factory();
        if (conn.State != ConnectionState.Open) conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private static List<PropertyInfo> GetMappedProps(Type type) =>
        type.GetProperties().Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() == null).ToList();

    private static void AddParam(IDbCommand cmd, string name, object? value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }
}
