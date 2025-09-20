using System.Data.SqlClient;
using AzureSqlCrudDemo.Data;
using AzureSqlCrudDemo.Models;
using Microsoft.EntityFrameworkCore;



if (args.Length > 0 && args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
{
    await TestConnectionAsync();
    return;
}

Console.WriteLine("== Azure SQL + EF Core: CRUD Demo ==");

using var db = new AppDbContext();


try
{
    await db.Database.MigrateAsync(); // Safe: applies pending migrations (if any)
    Console.WriteLine("Database is up-to-date (migrations applied).");
}
catch (Exception ex)
{
    Console.WriteLine("Migrate warning (you can ignore if you already updated DB via CLI): " + ex.Message);
}

// --- CREATE ---
var s1 = new Student { Name = "Alice",  Email = "alice@example.com" };
var s2 = new Student { Name = "Bob",    Email = "bob@example.com" };
db.Students.AddRange(s1, s2);
await db.SaveChangesAsync();
Console.WriteLine("Inserted 2 students.");

// --- READ (all) ---
var all = await db.Students.AsNoTracking().ToListAsync();
Console.WriteLine("Current Students:");
foreach (var s in all)
    Console.WriteLine($"  {s.Id}: {s.Name} <{s.Email}>");

// --- UPDATE ---
var bob = await db.Students.FirstAsync(x => x.Name == "Bob");
bob.Name = "Robert";
await db.SaveChangesAsync();
Console.WriteLine("Updated Bob → Robert.");

// --- READ (filter) ---
var roberts = await db.Students
    .AsNoTracking()
    .Where(s => s.Name!.Contains("Robert"))
    .ToListAsync();
Console.WriteLine("Filtered (Name contains 'Robert'):");
foreach (var s in roberts)
    Console.WriteLine($"  {s.Id}: {s.Name} <{s.Email}>");

// --- DELETE ---
var alice = await db.Students.FirstAsync(x => x.Name == "Alice");
db.Students.Remove(alice);
await db.SaveChangesAsync();
Console.WriteLine("Deleted Alice.");

// --- READ (final) ---
var finalAll = await db.Students.AsNoTracking().ToListAsync();
Console.WriteLine("Final Students:");
foreach (var s in finalAll)
    Console.WriteLine($"  {s.Id}: {s.Name} <{s.Email}>");

Console.WriteLine("== Done ==");

// ---------- helpers ----------
static async Task TestConnectionAsync()
{
    var connStr = Environment.GetEnvironmentVariable("AZURE_SQL_CONN");
    if (string.IsNullOrWhiteSpace(connStr))
    {
        Console.WriteLine("Set AZURE_SQL_CONN env var first.");
        return;
    }

    try
    {
        using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();
        Console.WriteLine("Connection OK!");
        await conn.CloseAsync();
        Console.WriteLine("Connection closed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("FAILED: " + ex.Message);
    }
}
