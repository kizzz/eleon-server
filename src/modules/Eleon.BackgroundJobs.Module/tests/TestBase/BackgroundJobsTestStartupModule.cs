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
using Volo.Abp.AutoMapper;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;
using BackgroundJobs.Module.BackgroundJobs;
using Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.HttpApi.Hubs.BackgroundJob;

namespace BackgroundJobs.Module.TestBase;

[DependsOn(
    typeof(BackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule),
    typeof(AbpAutoMapperModule)
)]
public class BackgroundJobsTestStartupModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();

        // Ensure AutoMapper is configured for the domain module
        // This matches what ModuleDomainModule does
        context.Services.AddAutoMapperObjectMapper<ModuleDomainModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<BackgroundJobsModuleDomainAutoMapperProfile>(validate: false);
        });

        // Add default configuration values for tests
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "BackgroundJobs", "true" }
        });
        var testConfiguration = configurationBuilder.Build();
        context.Services.AddSingleton<IConfiguration>(testConfiguration);

        var sqliteConnection = CreateDatabaseAndGetConnection();

        context.Services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(configurationContext =>
            {
                configurationContext.UseSqlite(sqliteConnection);
            });
        });

        // Register SignalR hub context mocks for integration tests
        var mockHubContext = Substitute.For<IHubContext<BackgroundJobHub, IBackgroundJobAppHubContext>>();
        var mockClients = Substitute.For<IHubClients<IBackgroundJobAppHubContext>>();
        var mockGroup = Substitute.For<IBackgroundJobAppHubContext>();
        mockHubContext.Clients.Returns(mockClients);
        mockClients.Group(NSubstitute.Arg.Any<string>()).Returns(mockGroup);
        context.Services.AddSingleton(mockHubContext);
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<VPortal.BackgroundJobs.Module.EntityFrameworkCore.BackgroundJobsDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new VPortal.BackgroundJobs.Module.EntityFrameworkCore.BackgroundJobsDbContext(options);
        dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
