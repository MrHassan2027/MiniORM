namespace MiniORM.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property)]
public class PrimaryKeyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class NotNullAttribute : Attribute { }
