using FallingDetectionService.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FallingDetectionService.Tests.Helpers;

public static class DbContextFactory
{
    public static (SafetyDbContext Db, SqliteConnection Connection) CreateSqliteInMemory()
    {
        // SQLite in-memory живе поки відкрите connection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<SafetyDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new SafetyDbContext(options);
        db.Database.EnsureCreated(); // створює schema по OnModelCreating

        return (db, connection);
    }
}