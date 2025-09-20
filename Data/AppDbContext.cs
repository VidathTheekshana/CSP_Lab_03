using AzureSqlCrudDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureSqlCrudDemo.Data;

public class AppDbContext : DbContext
{
    // Read the Azure SQL connection string from environment variable:
    //   setx AZURE_SQL_CONN "<your-connection-string>"
    private readonly string? _conn =
        Environment.GetEnvironmentVariable("AZURE_SQL_CONN");

    public DbSet<Student> Students => Set<Student>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (string.IsNullOrWhiteSpace(_conn))
        {
            throw new InvalidOperationException(
                "Set env var AZURE_SQL_CONN with your Azure SQL connection string.");
        }

        optionsBuilder.UseSqlServer(_conn);
    }
}
