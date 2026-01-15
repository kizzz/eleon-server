using Eleon.Logging.Lib.VportalLogging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using VPortal.JobScheduler.Module;
using VPortal.JobScheduler.Module.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Tasks;
using JobScheduler.Module.Tasks;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;

namespace JobScheduler.Module.TestBase;

[DependsOn(
    typeof(JobSchedulerEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule)
)]
public class JobSchedulerTestStartupModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();

        // Replace TaskHubContext with a no-op mock to avoid object mapper issues in tests
        context.Services.AddTransient<ITaskHubContext>(_ => Substitute.For<ITaskHubContext>());

        // Add default configuration values for tests
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JobScheduler", "true" }
        });
        var testConfiguration = configurationBuilder.Build();
        context.Services.AddSingleton<IConfiguration>(testConfiguration);

        var sqliteConnection = CreateDatabaseAndGetConnection();
        
        // Register the connection in DI so it can be disposed properly
        context.Services.AddSingleton(sqliteConnection);

        context.Services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(configurationContext =>
            {
                configurationContext.UseSqlite(sqliteConnection);
            });
        });

        Configure<AbpUnitOfWorkOptions>(options =>
        {
            options.IsTransactional = false; // Disable transactions for tests
        });

        // Register SignalR hub context mocks for integration tests
        var mockHubContext = Substitute.For<IHubContext<TaskHub, ITaskAppHubContext>>();
        var mockClients = Substitute.For<IHubClients<ITaskAppHubContext>>();
        var mockGroup = Substitute.For<ITaskAppHubContext>();
        mockHubContext.Clients.Returns(mockClients);
        mockClients.Group(NSubstitute.Arg.Any<string>()).Returns(mockGroup);
        context.Services.AddSingleton(mockHubContext);
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        var connection = context.ServiceProvider.GetService<SqliteConnection>();
        connection?.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<JobSchedulerDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new JobSchedulerDbContext(options);
        dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
