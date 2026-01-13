using Common.EventBus.Module;
using Eleon.Logging.Lib.VportalLogging;
using Eleon.Storage.Lib.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Eleon.TestsBase.Lib.TestHelpers;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Messages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using NSubstitute;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Autofac;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Security.Claims;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.FileManager.Module;
using VPortal.FileManager.Module.EntityFrameworkCore;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;
using System.IO;

namespace VPortal.FileManager.Module.Tests.TestBase;

[DependsOn(
    typeof(FileManagerEntityFrameworkCoreModule),
    typeof(ModuleApplicationModule),
    typeof(ModuleHttpApiModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule),
    typeof(AbpAutoMapperModule)
)]
public class FileManagerTestStartupModule : AbpModule
{
    public static Guid StorageProviderId { get; } = Guid.NewGuid();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        context.Services.AddAlwaysAllowAuthorization();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();

        var responseBus = EventBusTestHelpers.CreateMockResponseCapableEventBus();
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            responseBus,
            new GetStorageProviderResponseMsg
            {
                StorageProvider = new StorageProviderDto
                {
                    Id = StorageProviderId,
                    IsActive = true,
                    StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                    Settings = new List<StorageProviderSettingDto>
                    {
                        new StorageProviderSettingDto { Key = "BasePath", Value = Path.Combine(Path.GetTempPath(), "eleon-filemanager-tests") },
                        new StorageProviderSettingDto { Key = "IsMultitenancyEnabled", Value = "false" },
                        new StorageProviderSettingDto { Key = "AppendContainerNameToBasePath", Value = "false" }
                    }
                }
            });
        EventBusTestHelpers.SetupEventBusRequestAsync<GetMinimalStorageProvidersListMsg, GetMinimalStorageProvidersListResponseMsg>(
            responseBus,
            request => new GetMinimalStorageProvidersListResponseMsg
            {
                IsSuccess = true,
                StorageProviders = request.ProviderIds?.Select(id => new MinimalStorageProviderDto
                {
                    Id = id,
                    Name = $"Storage-{id}",
                    IsActive = true
                }).ToList() ?? new List<MinimalStorageProviderDto>()
            });

        context.Services.AddSingleton<IResponseCapableEventBus>(responseBus);
        context.Services.AddSingleton<IDistributedEventBus>(_ => (IDistributedEventBus)responseBus);
        var currentUser = TestMockHelpers.CreateMockCurrentUser(Guid.NewGuid());
        var currentTenant = TestMockHelpers.CreateMockCurrentTenant();
        var identityUser = new IdentityUser(currentUser.Id ?? Guid.NewGuid(), "testuser", "test@example.com", currentTenant.Id);
        var userManager = new MockIdentityUserManager(new Dictionary<Guid, IdentityUser>
        {
            { identityUser.Id, identityUser }
        });
        context.Services.Replace(ServiceDescriptor.Singleton<ICurrentUser>(currentUser));
        context.Services.Replace(ServiceDescriptor.Singleton<ICurrentTenant>(currentTenant));
        context.Services.Replace(ServiceDescriptor.Singleton<IdentityUserManager>(userManager));
        context.Services.Replace(ServiceDescriptor.Singleton<IdentityRoleManager>(new MockIdentityRoleManager()));
        var fakeVfsProvider = Substitute.For<IVfsBlobProvider>();
        fakeVfsProvider.GetAllBytesOrNullAsync(Arg.Any<VfsGetArgs>())
            .Returns(Task.FromResult<byte[]?>(new byte[] { 1, 2, 3 }));
        fakeVfsProvider.GetOrNullAsync(Arg.Any<VfsGetArgs>())
            .Returns(Task.FromResult<Stream?>(new MemoryStream(new byte[] { 1, 2, 3 })));
        fakeVfsProvider.SaveAsync(Arg.Any<VfsSaveArgs>()).Returns(Task.CompletedTask);
        fakeVfsProvider.ExistsAsync(Arg.Any<VfsExistArgs>()).Returns(Task.FromResult(true));
        fakeVfsProvider.TestAsync(Arg.Any<VfsTestArgs>()).Returns(Task.FromResult(true));
        fakeVfsProvider.ListAsync(Arg.Any<VfsListArgs>())
            .Returns(Task.FromResult<IReadOnlyList<VfsFileInfo>>(new List<VfsFileInfo>()));
        fakeVfsProvider.ListPagedAsync(Arg.Any<VfsListPagedArgs>())
            .Returns(Task.FromResult(new VfsPagedResult(new List<VfsFileInfo>(), 0)));
        fakeVfsProvider.DeleteAsync(Arg.Any<VfsDeleteArgs>()).Returns(Task.FromResult(true));

        context.Services.AddSingleton(sp =>
        {
            var cache = new VfsBlobProviderCacheService();
            var cacheKey = $"{currentTenant.Id};{StorageProviderId}";
            cache.Register(cacheKey, fakeVfsProvider);
            cache.Register("default", fakeVfsProvider);
            return cache;
        });
        context.Services.AddSingleton<VfsStorageProviderCacheManager>(sp =>
            new VfsStorageProviderCacheManager(
                Substitute.For<Microsoft.Extensions.Logging.ILogger<VfsStorageProviderCacheManager>>(),
                sp.GetRequiredService<IDistributedEventBus>(),
                sp.GetRequiredService<VfsBlobProviderCacheService>()));
        var blobContainer = Substitute.For<IBlobContainer>();
        context.Services.AddSingleton(blobContainer);
        context.Services.AddSingleton<IBlobContainerFactory>(_ =>
        {
            var factory = Substitute.For<IBlobContainerFactory>();
            factory.Create(Arg.Any<string>()).Returns(blobContainer);
            return factory;
        });
        context.Services.AddSingleton(new ContainersCacheDomainService(TestMockHelpers.CreateMockLogger<ContainersCacheDomainService>()));
        context.Services.AddSingleton(sp =>
            new StorageProviderOptionsManager(
                Substitute.For<IObjectMapper>(),
                sp.GetRequiredService<IDistributedEventBus>(),
                sp.GetRequiredService<ICurrentTenant>()));
        context.Services.AddSingleton(sp =>
            new StorageDomainService(
                TestMockHelpers.CreateMockLogger<StorageDomainService>(),
                sp.GetRequiredService<IBlobContainerFactory>(),
                sp.GetRequiredService<ContainersCacheDomainService>(),
                sp.GetRequiredService<IDistributedEventBus>(),
                sp.GetRequiredService<StorageProviderOptionsManager>()));

        // Ensure AutoMapper is configured for the domain and application modules
        context.Services.AddAutoMapperObjectMapper<ModuleDomainModule>();
        context.Services.AddAutoMapperObjectMapper<ModuleApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ModuleDomainModule>(validate: false);
            options.AddMaps<ModuleApplicationModule>(validate: false);
        });

        // Add default configuration values for tests
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "FileManager", "true" }
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

        // Register mock identity managers for integration tests
        // Identity managers already registered above with seeded user
        context.Services.AddSingleton<IOrganizationUnitRepository>(_ => Substitute.For<IOrganizationUnitRepository>());
        context.Services.AddSingleton<OrganizationUnitManager>(sp =>
        {
            var orgUnitRepository = sp.GetRequiredService<IOrganizationUnitRepository>();
            var localizer = Substitute.For<IStringLocalizer<IdentityResource>>();
            var roleRepository = Substitute.For<IIdentityRoleRepository>();
            var cache = Substitute.For<IDistributedCache<AbpDynamicClaimCacheItem>>();
            var cancellationTokenProvider = Substitute.For<ICancellationTokenProvider>();

            return new OrganizationUnitManager(
                orgUnitRepository,
                localizer,
                roleRepository,
                cache,
                cancellationTokenProvider);
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FileManagerDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new FileManagerDbContext(options);
        dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
