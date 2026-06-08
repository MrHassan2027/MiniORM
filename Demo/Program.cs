using Microsoft.Data.Sqlite;
using MiniORM;
using MiniORM.Attributes;

var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
var db = new Database(() => connection);

db.CreateTable<User>();

db.Insert(new User { Name = "Hassan", Email = "hassan@example.com", Age = 22 });
db.Insert(new User { Name = "Ali",    Email = "ali@example.com",    Age = 25 });
db.Insert(new User { Name = "Sara",   Email = "sara@example.com",   Age = 20 });

Console.WriteLine("All users:");
var all = db.Query<User>();
foreach (var u in all)
    Console.WriteLine($"  [{u.Id}] {u.Name} | {u.Email} | age {u.Age}");

Console.WriteLine("\nUsers older than 21:");
var filtered = db.Query<User>(u => u.Age > 21);
foreach (var u in filtered)
    Console.WriteLine($"  {u.Name} (age {u.Age})");

Console.WriteLine("\nDone.");

[Table("users")]
class User
{
    [PrimaryKey] public int Id { get; set; }
    [Column("name")][NotNull] public string Name { get; set; } = "";
    [Column("email")] public string Email { get; set; } = "";
    [Column("age")] public int Age { get; set; }
}
