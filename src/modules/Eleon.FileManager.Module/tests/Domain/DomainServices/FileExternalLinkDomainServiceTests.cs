using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Commons.Module.Messages.Features;
using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.BlobStoring;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Managers;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

public class FileExternalLinkDomainServiceTests : DomainTestBase
{
    private readonly FileExternalLinkDomainService _service;
    private readonly FileArchivePermissionCheckerDomainService _permissionChecker;
    private readonly FileEditManager _fileEditManager;
    private readonly VPortal.FileManager.Module.Managers.FileManager _fileManager;
    private readonly IFileFactory _fileFactory;
    private readonly IFileSystemEntryRepository _fileSystemEntryRepository;
    private readonly FileStatusDomainService _fileStatusDomainService;
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentUser _currentUser;
    private readonly Guid _currentUserId;
    private readonly IFileExternalLinkRepository _repository;
    private readonly IArchiveRepository _archiveRepository;
    private readonly IDistributedEventBus _getRequestClient;
    private readonly IDistributedEventBus _createRequestClient;
    private readonly IDistributedEventBus _updateRequestClient;
    private readonly IDistributedEventBus _massTransitPublisher;
    private readonly IdentityRoleManager _identityRoleManager;
    private readonly OrganizationUnitManager _organizationUnitManager;
    private readonly UnitOfWorkManager _unitOfWorkManager;
    private readonly IOrganizationUnitRepository _orgUnitRepository;

    public FileExternalLinkDomainServiceTests()
    {
        _currentUserId = Guid.NewGuid();
        _currentUser = CreateMockCurrentUser(_currentUserId);

        _archiveRepository = CreateMockArchiveRepository();
        _fileFactory = CreateMockFileFactory();
        _fileSystemEntryRepository = CreateMockFileSystemEntryRepository();
        _fileFactory.Get(Arg.Any<FileArchiveEntity>(), Arg.Any<FileManagerType>())
            .Returns(_fileSystemEntryRepository);
        _archiveRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo => FileManagerTestDataBuilder.FileArchive()
                .WithId(callInfo.Arg<Guid>())
                .Build());

        var permissionManagementService = new FileArchivePermissionManagementDomainService(
            CreateMockFileArchivePermissionRepository(),
            CreateMockLogger<FileArchivePermissionManagementDomainService>(),
            new MockIdentityUserManager(),
            new MockIdentityRoleManager(),
            CreateMockOrganizationUnitRepository(),
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource>>());

        _fileManager = new VPortal.FileManager.Module.Managers.FileManager(
            CreateMockLogger<VPortal.FileManager.Module.Managers.FileManager>(),
            _fileFactory,
            _archiveRepository);

        _permissionChecker = new FileArchivePermissionCheckerDomainService(
            permissionManagementService,
            _currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: true, userId: _currentUserId),
            _archiveRepository);

        var responseBus = CreateMockResponseCapableEventBus();
        SetupEventBusRequestAsync<GetFeatureSettingMsg, GetFeatureSettingResponseMsg>(
            responseBus,
            new GetFeatureSettingResponseMsg { Value = string.Empty });

        var storageProviderOptionsManager = new StorageProviderOptionsManager(
            CreateMockObjectMapper(),
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)responseBus,
            CreateMockCurrentTenant());

        var blobContainer = Substitute.For<IBlobContainer>();
        var blobContainerFactory = Substitute.For<IBlobContainerFactory>();
        blobContainerFactory.Create(Arg.Any<string>()).Returns(blobContainer);
        var containersCache = new ContainersCacheDomainService(CreateMockLogger<ContainersCacheDomainService>());

        var storageDomainService = new StorageDomainService(
            CreateMockLogger<StorageDomainService>(),
            blobContainerFactory,
            containersCache,
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)responseBus,
            storageProviderOptionsManager);
        SetCurrentTenant(storageDomainService, CreateMockCurrentTenant());

        _fileEditManager = new FileEditManager(
            CreateMockLogger<FileEditManager>(),
            storageDomainService,
            storageProviderOptionsManager);

        _fileStatusDomainService = Substitute.For<FileStatusDomainService>(
            CreateMockLogger<FileStatusDomainService>(),
            CreateMockFileStatusRepository());

        _identityUserManager = SecurityTestHelpers.CreateMockIdentityUserManager();
        _repository = CreateMockFileExternalLinkRepository();
        _getRequestClient = CreateMockEventBus();
        _createRequestClient = CreateMockEventBus();
        _updateRequestClient = CreateMockEventBus();
        _massTransitPublisher = CreateMockEventBus();
        _identityRoleManager = new MockIdentityRoleManager();

        _orgUnitRepository = Substitute.For<IOrganizationUnitRepository>();
        _organizationUnitManager = new OrganizationUnitManager(
            _orgUnitRepository,
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<IdentityResource>>(),
            Substitute.For<Volo.Abp.Identity.IIdentityRoleRepository>(),
            Substitute.For<Volo.Abp.Caching.IDistributedCache<Volo.Abp.Security.Claims.AbpDynamicClaimCacheItem>>(),
            Substitute.For<Volo.Abp.Threading.ICancellationTokenProvider>());

        _unitOfWorkManager = null!;

        _service = new FileExternalLinkDomainService(
            CreateMockLogger<FileExternalLinkDomainService>(),
            _permissionChecker,
            _fileEditManager,
            _fileManager,
            _fileStatusDomainService,
            _identityUserManager,
            _currentUser,
            _getRequestClient,
            _archiveRepository,
            _createRequestClient,
            _updateRequestClient,
            _massTransitPublisher,
            _identityRoleManager,
            _organizationUnitManager,
            _unitOfWorkManager,
            _orgUnitRepository,
            _repository);
    }

    private static void SetCurrentTenant(Volo.Abp.Domain.Services.DomainService service, ICurrentTenant currentTenant)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>().Returns(currentTenant);

        var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
            "LazyServiceProvider",
            BindingFlags.Public | BindingFlags.Instance);
        lazyServiceProviderProp?.SetValue(service, lazyServiceProvider);
    }

    #region GetLinksAsync Tests

    [Fact]
    public async Task GetLinksAsync_ValidArchive_ReturnsLinks()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileShareStatuses = new List<FileShareStatus> { FileShareStatus.Readonly };
        var links = new List<FileExternalLinkEntity>
        {
            FileManagerTestDataBuilder.FileExternalLink()
                .WithArchiveId(archiveId)
                .WithPermissionType(FileShareStatus.Readonly)
                .Build()
        };

        _repository.GetListAsync(Arg.Any<bool>()).Returns(links);

        // Act
        var result = await _service.GetLinksAsync(archiveId, fileShareStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetLinksAsync_NoLinks_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileShareStatuses = new List<FileShareStatus> { FileShareStatus.Readonly };

        _repository.GetListAsync(Arg.Any<bool>()).Returns(new List<FileExternalLinkEntity>());

        // Act
        var result = await _service.GetLinksAsync(archiveId, fileShareStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLinksAsync_MultipleStatuses_ReturnsFilteredLinks()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileShareStatuses = new List<FileShareStatus> { FileShareStatus.Readonly, FileShareStatus.Modify };
        var links = new List<FileExternalLinkEntity>
        {
            FileManagerTestDataBuilder.FileExternalLink()
                .WithArchiveId(archiveId)
                .WithPermissionType(FileShareStatus.Readonly)
                .Build(),
            FileManagerTestDataBuilder.FileExternalLink()
                .WithArchiveId(archiveId)
                .WithPermissionType(FileShareStatus.Modify)
                .Build(),
            FileManagerTestDataBuilder.FileExternalLink()
                .WithArchiveId(archiveId)
                .WithPermissionType(FileShareStatus.None)
                .Build()
        };

        _repository.GetListAsync(Arg.Any<bool>()).Returns(links);

        // Act
        var result = await _service.GetLinksAsync(archiveId, fileShareStatuses);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(l => fileShareStatuses.Contains(l.PermissionType));
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_ValidFile_ReturnsLink()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var file = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();
        var link = FileManagerTestDataBuilder.FileExternalLink()
            .WithArchiveId(archiveId)
            .WithFileId(fileId)
            .Build();

        _fileSystemEntryRepository.GetEntryById(fileId).Returns(file);

        _repository.GetListAsync(true).Returns(new List<FileExternalLinkEntity> { link });

        // Act
        var result = await _service.GetAsync(fileId, archiveId);

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().Be(fileId);
        result.ArchiveId.Should().Be(archiveId);
    }

    [Fact]
    public async Task GetAsync_NoPermission_ReturnsNull()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        _fileSystemEntryRepository.GetEntryById(fileId).Returns(FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build());

        var permissionRepository = CreateMockFileArchivePermissionRepository();
        permissionRepository.GetListAsync(archiveId, Arg.Any<List<string>>())
            .Returns(new List<FileArchivePermissionEntity>());
        var permissionManagementService = new FileArchivePermissionManagementDomainService(
            permissionRepository,
            CreateMockLogger<FileArchivePermissionManagementDomainService>(),
            new MockIdentityUserManager(),
            new MockIdentityRoleManager(),
            CreateMockOrganizationUnitRepository(),
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource>>());
        var nonAdminUserManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: _currentUserId);
        var permissionChecker = new FileArchivePermissionCheckerDomainService(
            permissionManagementService,
            _currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            nonAdminUserManager,
            _archiveRepository);
        var service = new FileExternalLinkDomainService(
            CreateMockLogger<FileExternalLinkDomainService>(),
            permissionChecker,
            _fileEditManager,
            _fileManager,
            _fileStatusDomainService,
            _identityUserManager,
            _currentUser,
            _getRequestClient,
            _archiveRepository,
            _createRequestClient,
            _updateRequestClient,
            _massTransitPublisher,
            _identityRoleManager,
            _organizationUnitManager,
            _unitOfWorkManager,
            _orgUnitRepository,
            _repository);

        // Act
        var result = await service.GetAsync(fileId, archiveId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_LinkNotFound_ReturnsNewLink()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var file = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();

        _fileSystemEntryRepository.GetEntryById(fileId).Returns(file);

        _repository.GetListAsync(true).Returns(new List<FileExternalLinkEntity>());

        // Act
        var result = await _service.GetAsync(fileId, archiveId);

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().Be(fileId);
        result.ArchiveId.Should().Be(archiveId);
        result.PermissionType.Should().Be(FileShareStatus.None);
        result.Reviewers.Should().NotBeNull();
        result.Reviewers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_FileNotFound_ReturnsNull()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        _fileSystemEntryRepository.GetEntryById(fileId).Returns((FileSystemEntry)null);

        // Act
        var result = await _service.GetAsync(fileId, archiveId);

        // Assert - Exception is caught and logged, returns null
        result.Should().BeNull();
    }

    #endregion
}
