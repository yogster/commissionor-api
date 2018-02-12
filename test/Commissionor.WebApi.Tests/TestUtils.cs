using System.Threading.Tasks;
using Commissionor.WebApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Commissionor.WebApi.Tests
{
    static class TestUtils
    {
        public static async Task<CommissionorDbContext> CreateTestDb()
        {
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();

            var optionsBuilder = new DbContextOptionsBuilder<CommissionorDbContext>();
            optionsBuilder.UseSqlite(inMemorySqlite);

            var dbContext = new CommissionorDbContext(optionsBuilder.Options);
            await dbContext.Database.MigrateAsync();
            return dbContext;
        }
    }
}
