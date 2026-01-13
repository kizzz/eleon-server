using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Migrations.Module;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using NSubstitute;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Managers;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

public class FileArchivePermissionCheckerDomainServiceTests : DomainTestBase
{
    private readonly FileArchivePermissionCheckerDomainService _service;
    private readonly FileArchivePermissionManagementDomainService _permissionManagementService;
    private readonly VPortal.FileManager.Module.Managers.FileManager _fileManager;
    private readonly IFileFactory _fileFactory;
    private readonly IFileSystemEntryRepository _fileSystemEntryRepository;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IArchiveRepository _archiveRepository;
    private readonly IFileArchivePermissionRepository _permissionRepository;
    private readonly ICurrentUser _currentUser;
    private readonly Guid _currentUserId;

    public FileArchivePermissionCheckerDomainServiceTests()
    {
        _permissionRepository = CreateMockFileArchivePermissionRepository();
        _permissionManagementService = new FileArchivePermissionManagementDomainService(
            _permissionRepository,
            CreateMockLogger<FileArchivePermissionManagementDomainService>(),
            new MockIdentityUserManager(),
            new MockIdentityRoleManager(),
            CreateMockOrganizationUnitRepository(),
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource>>());

        _archiveRepository = CreateMockArchiveRepository();
        _fileFactory = CreateMockFileFactory();
        _fileSystemEntryRepository = CreateMockFileSystemEntryRepository();
        _fileFactory.Get(Arg.Any<FileArchiveEntity>(), Arg.Any<FileManagerType>())
            .Returns(_fileSystemEntryRepository);
        _fileSystemEntryRepository.GetEntryParentsById(Arg.Any<string>())
            .Returns(callInfo =>
            {
                var folderId = callInfo.Arg<string>();
                return Task.FromResult(new List<FileSystemEntry> { new FileSystemEntry(folderId) });
            });

        _fileManager = new VPortal.FileManager.Module.Managers.FileManager(
            CreateMockLogger<VPortal.FileManager.Module.Managers.FileManager>(),
            _fileFactory,
            _archiveRepository);

        _currentUserId = Guid.NewGuid();
        _currentUser = CreateMockCurrentUser(_currentUserId);
        _identityUserManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: _currentUserId);
        _service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            _currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            _identityUserManager,
            _archiveRepository);
    }

    private void SetupPermissions(Guid archiveId, string folderId, IEnumerable<FileManagerPermissionType> permissionTypes, Guid? actorId = null)
    {
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .Build();

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(archive);

        List<FileArchivePermissionEntity> permissions = null;
        var effectiveActorId = (actorId ?? _currentUserId).ToString();
        if (permissionTypes != null)
        {
            permissions = permissionTypes
                .Select(permissionType => FileManagerTestDataBuilder.FileArchivePermission()
                    .WithArchiveId(archiveId)
                    .WithFolderId(folderId)
                    .WithActorType(PermissionActorType.User)
                    .WithActorId(effectiveActorId)
                    .WithPermissionType(permissionType)
                    .Build())
                .ToList();
        }

        _permissionRepository.GetListAsync(archiveId, Arg.Any<List<string>>())
            .Returns(permissions);
    }

    #region CheckPermission Tests

    [Fact]
    public async Task CheckPermission_ReadAllowed_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Read };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_ReadDenied_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Write };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckPermission_WriteAllowed_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Write };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Write, FileManagerType.FileArchive);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_ModifyAllowed_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Modify };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Modify, FileManagerType.FileArchive);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_AdminUser_ReturnsAllPermissions()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .Build();
        var currentUser = CreateMockCurrentUser(userId);
        var adminUserManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: true, userId: userId);
        
        var service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            adminUserManager,
            _archiveRepository);

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(archive);

        // Act
        var readResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);
        var writeResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Write, FileManagerType.FileArchive);
        var modifyResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Modify, FileManagerType.FileArchive);

        // Assert
        readResult.Should().BeTrue();
        writeResult.Should().BeTrue();
        modifyResult.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_ArchiveCreator_ReturnsAllPermissions()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();
        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .Build();
        archive.CreatorId = userId;

        var currentUser = CreateMockCurrentUser(userId);
        var userManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: userId);
        var service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            userManager,
            _archiveRepository);

        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(archive);

        // Act
        var readResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);
        var writeResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Write, FileManagerType.FileArchive);
        var modifyResult = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Modify, FileManagerType.FileArchive);

        // Assert
        readResult.Should().BeTrue();
        writeResult.Should().BeTrue();
        modifyResult.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_ProviderType_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.Provider);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_NoPermissions_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var permissions = new List<FileManagerPermissionType>();

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckPermission_NullPermissions_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();

        SetupPermissions(archiveId, folderId, null);

        // Act
        var result = await _service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region CheckFilePermission Tests

    [Fact]
    public async Task CheckFilePermission_ValidFile_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var file = FileManagerTestDataBuilder.FileSystemEntry()
            .WithId(fileId)
            .WithArchiveId(archiveId)
            .AsFile()
            .Build();
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Read };

        _fileSystemEntryRepository.GetEntryById(fileId).Returns(file);
        SetupPermissions(archiveId, file.FolderId, permissions);

        // Act
        var result = await _service.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckFilePermission_FileNotFound_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        var archive = FileManagerTestDataBuilder.FileArchive()
            .WithId(archiveId)
            .Build();
        _archiveRepository.GetAsync(archiveId, Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(archive);
        _fileSystemEntryRepository.GetEntryById(fileId).Returns((FileSystemEntry)null);

        // Act
        var result = await _service.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckFilePermission_ProviderType_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();

        // Act
        var result = await _service.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Read, FileManagerType.Provider);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetPermited Tests

    [Fact]
    public async Task GetPermited_FiltersByPermission_ReturnsFilteredList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var entries = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(folderId)
                .AsFile()
                .Build(),
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(folderId)
                .AsFile()
                .Build()
        };
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Read };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.GetPermited(archiveId, entries, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPermited_ProviderType_ReturnsAllEntries()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var entries = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .AsFile()
                .Build()
        };

        // Act
        var result = await _service.GetPermited(archiveId, entries, FileManagerPermissionType.Read, FileManagerType.Provider);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(entries);
    }

    [Fact]
    public async Task GetPermited_NoPermission_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var entries = new List<FileSystemEntry>
        {
            FileManagerTestDataBuilder.FileSystemEntry()
                .WithId(Guid.NewGuid().ToString())
                .WithArchiveId(archiveId)
                .WithParentId(folderId)
                .AsFile()
                .Build()
        };
        var permissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Write };

        SetupPermissions(archiveId, folderId, permissions);

        // Act
        var result = await _service.GetPermited(archiveId, entries, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetAllowedPermissionsForCurrentUser Tests

    [Fact]
    public async Task GetAllowedPermissionsForCurrentUser_UserWithPermissions_ReturnsPermissions()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();

        var currentUser = CreateMockCurrentUser(userId);
        var userManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: userId);
        
        var service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            userManager,
            _archiveRepository);

        SetupPermissions(archiveId, folderId, new[] { FileManagerPermissionType.Read }, userId);

        // Act
        var result = await service.GetAllowedPermissionsForCurrentUser(archiveId, folderId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(FileManagerPermissionType.Read);
    }

    [Fact]
    public async Task GetAllowedPermissionsForCurrentUser_UserWithoutPermissions_ReturnsEmpty()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();

        var currentUser = CreateMockCurrentUser(userId);
        var userManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: userId);
        
        var service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            currentUser,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            userManager,
            _archiveRepository);

        SetupPermissions(archiveId, folderId, Array.Empty<FileManagerPermissionType>(), userId);

        // Act
        var result = await service.GetAllowedPermissionsForCurrentUser(archiveId, folderId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region Security Tests

    [Fact]
    public async Task CheckPermission_CrossTenantAccess_Denied()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var tenant1Id = Guid.NewGuid();
        var tenant2Id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenant1User = CreateMockCurrentUser(userId, tenantId: tenant1Id);
        var userManager = SecurityTestHelpers.CreateMockIdentityUserManager(isAdmin: false, userId: userId);
        var service = new FileArchivePermissionCheckerDomainService(
            _permissionManagementService,
            tenant1User,
            _fileManager,
            CreateMockLogger<FileArchivePermissionCheckerDomainService>(),
            userManager,
            _archiveRepository);

        SetupPermissions(archiveId, folderId, Array.Empty<FileManagerPermissionType>(), userId);

        // Act
        var result = await service.CheckPermission(archiveId, folderId, FileManagerPermissionType.Read, FileManagerType.FileArchive);

        // Assert - Should be denied if no permissions
        result.Should().BeFalse();
    }

    #endregion
}
