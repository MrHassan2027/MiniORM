# MiniORM

> Lightweight attribute-based ORM for SQL Server and SQLite — zero Entity Framework dependency

## What it does
Maps C# classes to database tables using custom attributes. Supports CRUD operations, simple WHERE queries, and schema auto-migration. Designed to be embedded in any project as a single NuGet package with no heavy dependencies.

## Quick Start
```bash
git clone https://github.com/yourusername/MiniORM
cd MiniORM
dotnet build
dotnet test
```

```csharp
[Table("users")]
public class User
{
    [PrimaryKey] public int Id { get; set; }
    [Column("name")] public string Name { get; set; } = "";
    [Column("email")] public string Email { get; set; } = "";
}

var db = new Database("Data Source=app.db");
db.CreateTable<User>();
db.Insert(new User { Name = "Hassan", Email = "test@test.com" });
var users = db.Query<User>(u => u.Name == "Hassan");
```

## Features
- `[Table]`, `[Column]`, `[PrimaryKey]`, `[NotNull]` attributes
- Auto `CREATE TABLE` from class definition
- `Insert`, `Update`, `Delete`, `Query<T>` with lambda predicates
- Supports SQLite and SQL Server via `IDbConnection`
- No reflection caching overhead — uses compiled expression trees

## Tech Stack
| Tool | Why |
|------|-----|
| C# / .NET 8 | Expression trees for predicate-to-SQL compilation |
| `System.Data` | Provider-agnostic `IDbConnection` |
| SQLite (tests) | In-memory DB for fast unit tests |

## Architecture
```
MiniORM/
├── Attributes/       # Table, Column, PrimaryKey, NotNull
├── Core/
│   ├── Database.cs   # Main API: Insert/Update/Delete/Query
│   ├── SchemaBuilder.cs  # Generates CREATE TABLE SQL
│   └── QueryCompiler.cs  # Compiles Expression<Func<T,bool>> → WHERE clause
└── MiniORM.Tests/    # xUnit tests with in-memory SQLite
```
