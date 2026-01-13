using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VPortal.Identity.Module.EntityFrameworkCore;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.EntityFrameworkCore;

public class IdentityDbContextTests
{
    [Fact]
    public void CanCreateDatabaseInMemory()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new IdentityDbContext(options);
        context.Database.EnsureCreated();
    }
}
