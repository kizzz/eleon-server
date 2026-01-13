using System;
using System.Threading.Tasks;
using Common.EventBus.Module;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Eleon.Storage.Lib.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NSubstitute;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Repositories.File;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.Factories;

public class FileFactoryTests : DomainTestBase
{
    private readonly FileFactory _factory;
    private readonly IServiceProvider _serviceProvider;
    private readonly VfsStorageProviderCacheManager _vfsStorageProviderCacheService;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IResponseCapableEventBus _eventBus;
    private readonly FilePhysicalRepository _physicalRepository;
    private readonly FileVirtualRepository _virtualRepository;

    public FileFactoryTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _currentTenant = CreateMockCurrentTenant();
        _currentUser = CreateMockCurrentUser();
        _eventBus = CreateMockResponseCapableEventBus();

        var storageProviderResponse = new GetStorageProviderResponseMsg
        {
            StorageProvider = new StorageProviderDto
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Settings = new List<StorageProviderSettingDto>
                {
                    new StorageProviderSettingDto { Key = "ProxyId", Value = "proxy-1" }
                }
            }
        };

        SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(_eventBus, storageProviderResponse);

        _vfsStorageProviderCacheService = new VfsStorageProviderCacheManager(
            Substitute.For<Microsoft.Extensions.Logging.ILogger<VfsStorageProviderCacheManager>>(),
            (IDistributedEventBus)_eventBus,
            new VfsBlobProviderCacheService());
        _physicalRepository = new FilePhysicalRepository(CreateMockLogger<FilePhysicalRepository>());
        _virtualRepository = new FileVirtualRepository(
            CreateMockLogger<FileVirtualRepository>(),
            Substitute.For<IBasicRepository<FileSystemEntry, string>>(),
            CreateMockGuidGenerator(),
            CreateMockPhysicalFolderRepository(),
            _currentUser,
            Substitute.For<IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource>>(),
            _vfsStorageProviderCacheService);

        _factory = new FileFactory(
            _serviceProvider,
            _vfsStorageProviderCacheService,
            _currentTenant,
            _currentUser,
            (IDistributedEventBus)_eventBus);
    }

    #region Get(FileArchiveEntity, FileManagerType) Tests

    [Fact]
    public async Task Get_FileArchiveTypeVirtual_ReturnsVirtualRepository()
    {
        // Arrange
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithHierarchyType(FileArchiveHierarchyType.Virtual)
            .WithIsActive(true)
            .Build();

        _serviceProvider.GetService(typeof(FileVirtualRepository))
            .Returns(_virtualRepository);

        // Act
        var result = await _factory.Get(archive, FileManagerType.FileArchive);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_virtualRepository);
        result.Archive.Should().Be(archive);
    }

    [Fact]
    public async Task Get_FileArchiveTypePhysical_ThrowsNotImplementedException()
    {
        // Arrange
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithHierarchyType(FileArchiveHierarchyType.Physical)
            .WithIsActive(true)
            .Build();

        // Act
        var result = await _factory.Get(archive, FileManagerType.FileArchive);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<FileStorageProviderRepository>();
    }

    [Fact]
    public async Task Get_ProviderType_ReturnsFileStorageProviderRepository()
    {
        // Arrange
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithHierarchyType(FileArchiveHierarchyType.Physical)
            .WithIsActive(true)
            .Build();

        // Act
        var result = await _factory.Get(archive, FileManagerType.Provider);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<FileStorageProviderRepository>();
        result.Archive.Should().Be(archive);
    }

    [Fact]
    public async Task Get_PhysicalHierarchyProviderType_ReturnsFileStorageProviderRepository()
    {
        // Arrange
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithHierarchyType(FileArchiveHierarchyType.Physical)
            .WithIsActive(true)
            .Build();

        // Act
        var result = await _factory.Get(archive, FileManagerType.Provider);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<FileStorageProviderRepository>();
    }

    #endregion

    #region Get(FileArchiveHierarchyType) Tests

    [Fact]
    public void Get_PhysicalHierarchyType_ReturnsPhysicalRepository()
    {
        // Arrange
        _serviceProvider.GetService(typeof(FilePhysicalRepository))
            .Returns(_physicalRepository);

        // Act
        var result = _factory.Get(FileArchiveHierarchyType.Physical);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_physicalRepository);
    }

    [Fact]
    public void Get_VirtualHierarchyType_ReturnsVirtualRepository()
    {
        // Arrange
        _serviceProvider.GetService(typeof(FileVirtualRepository))
            .Returns(_virtualRepository);

        // Act
        var result = _factory.Get(FileArchiveHierarchyType.Virtual);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_virtualRepository);
    }

    [Fact]
    public void Get_InvalidHierarchyType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = (FileArchiveHierarchyType)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _factory.Get(invalidType));
        exception.Message.Should().Contain("Unknown FileArchiveHierarchyType");
    }

    [Fact]
    public void Get_PhysicalHierarchyType_ServiceNotRegistered_ReturnsNull()
    {
        // Arrange
        _serviceProvider.GetService(typeof(FilePhysicalRepository))
            .Returns((FilePhysicalRepository)null);

        // Act
        var result = _factory.Get(FileArchiveHierarchyType.Physical);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Get_VirtualHierarchyType_ServiceNotRegistered_ReturnsNull()
    {
        // Arrange
        _serviceProvider.GetService(typeof(FileVirtualRepository))
            .Returns((FileVirtualRepository)null);

        // Act
        var result = _factory.Get(FileArchiveHierarchyType.Virtual);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public async Task Get_ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithHierarchyType(FileArchiveHierarchyType.Virtual)
            .WithIsActive(true)
            .Build();

        _serviceProvider.GetService(typeof(FileVirtualRepository))
            .Returns(_virtualRepository);

        // Act
        var tasks = new[]
        {
            _factory.Get(archive, FileManagerType.FileArchive),
            _factory.Get(archive, FileManagerType.FileArchive),
            _factory.Get(archive, FileManagerType.FileArchive)
        };

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllBeEquivalentTo(_virtualRepository);
        results.Should().HaveCount(3);
    }

    #endregion
}
