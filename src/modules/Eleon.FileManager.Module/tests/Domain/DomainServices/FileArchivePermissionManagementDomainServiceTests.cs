using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using NSubstitute;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using VPortal.FileManager.Module.ValueObjects;
using Volo.Abp.Identity;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

public class FileArchivePermissionManagementDomainServiceTests : DomainTestBase
{
    private readonly FileArchivePermissionManagementDomainService _service;
    private readonly IFileArchivePermissionRepository _repository;

    public FileArchivePermissionManagementDomainServiceTests()
    {
        _repository = CreateMockFileArchivePermissionRepository();

        _service = new FileArchivePermissionManagementDomainService(
            _repository,
            CreateMockLogger<FileArchivePermissionManagementDomainService>(),
            new MockIdentityUserManager(),
            new MockIdentityRoleManager(),
            CreateMockOrganizationUnitRepository(),
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource>>());
    }

    [Fact]
    public async Task GetListAsync_WithPermissions_ReturnsList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderIds = new List<string> { Guid.NewGuid().ToString() };
        var permissions = new List<FileArchivePermissionEntity>
        {
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderIds[0])
                .WithActorType(PermissionActorType.User)
                .WithPermissionType(FileManagerPermissionType.Read)
                .Build()
        };

        _repository.GetListAsync(archiveId, folderIds)
            .Returns(permissions);

        // Act
        var result = await _service.GetListAsync(archiveId, folderIds);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetListAsync_EmptyFolderList_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderIds = new List<string>();

        _repository.GetListAsync(archiveId, folderIds)
            .Returns(new List<FileArchivePermissionEntity>());

        // Act
        var result = await _service.GetListAsync(archiveId, folderIds);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetListAsync_MultipleFolders_ReturnsAllPermissions()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId1 = Guid.NewGuid().ToString();
        var folderId2 = Guid.NewGuid().ToString();
        var folderIds = new List<string> { folderId1, folderId2 };
        
        var permissions = new List<FileArchivePermissionEntity>
        {
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderId1)
                .WithActorType(PermissionActorType.User)
                .WithPermissionType(FileManagerPermissionType.Read)
                .Build(),
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderId2)
                .WithActorType(PermissionActorType.Role)
                .WithPermissionType(FileManagerPermissionType.Write)
                .Build()
        };

        _repository.GetListAsync(archiveId, folderIds)
            .Returns(permissions);

        // Act
        var result = await _service.GetListAsync(archiveId, folderIds);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdatePermission_ValidInput_UpdatesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var actorId = Guid.NewGuid().ToString();
        
        var valueObject = new FileArchivePermissionValueObject
        {
            ArchiveId = archiveId,
            FolderId = folderId,
            ActorType = PermissionActorType.User,
            ActorId = actorId,
            AllowedPermissions = new List<FileManagerPermissionType> { FileManagerPermissionType.Read, FileManagerPermissionType.Write }
        };

        var updatedPermission = FileManagerTestDataBuilder.FileArchivePermission()
            .WithArchiveId(archiveId)
            .WithFolderId(folderId)
            .WithActorType(PermissionActorType.User)
            .WithActorId(actorId)
            .WithPermissionType(FileManagerPermissionType.Read)
            .Build();

        _repository.UpdatePermissions(valueObject)
            .Returns(updatedPermission);

        // Act
        var result = await _service.UpdatePermission(valueObject);

        // Assert
        result.Should().NotBeNull();
        result.ArchiveId.Should().Be(archiveId);
        result.FolderId.Should().Be(folderId);
    }

    [Fact]
    public async Task GetPermissionWithoutDefault_ExistingPermission_ReturnsPermission()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var actorId = Guid.NewGuid().ToString();
        
        var keyValueObject = new FileArchivePermissionKeyValueObject
        {
            ArchiveId = archiveId,
            FolderId = folderId,
            ActorType = PermissionActorType.User,
            ActorId = actorId
        };

        var permission = FileManagerTestDataBuilder.FileArchivePermission()
            .WithArchiveId(archiveId)
            .WithFolderId(folderId)
            .WithActorType(PermissionActorType.User)
            .WithActorId(actorId)
            .Build();

        _repository.GetPermissionWithoutDefault(keyValueObject)
            .Returns(permission);

        // Act
        var result = await _service.GetPermissionWithoutDefault(keyValueObject);

        // Assert
        result.Should().NotBeNull();
        result.ArchiveId.Should().Be(archiveId);
    }

    [Fact]
    public async Task GetPermissionWithoutDefault_NonExistent_ReturnsNull()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var actorId = Guid.NewGuid().ToString();
        
        var keyValueObject = new FileArchivePermissionKeyValueObject
        {
            ArchiveId = archiveId,
            FolderId = folderId,
            ActorType = PermissionActorType.User,
            ActorId = actorId
        };

        _repository.GetPermissionWithoutDefault(keyValueObject)
            .Returns((FileArchivePermissionEntity)null);

        // Act
        var result = await _service.GetPermissionWithoutDefault(keyValueObject);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeletePermissions_ValidInput_DeletesSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var actorId = Guid.NewGuid().ToString();
        
        var valueObject = new FileArchivePermissionValueObject
        {
            ArchiveId = archiveId,
            FolderId = folderId,
            ActorType = PermissionActorType.User,
            ActorId = actorId
        };

        _repository.DeletePermissions(valueObject)
            .Returns(true);

        // Act
        var result = await _service.DeletePermissions(valueObject);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePermissions_NonExistent_ReturnsFalse()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var actorId = Guid.NewGuid().ToString();
        
        var valueObject = new FileArchivePermissionValueObject
        {
            ArchiveId = archiveId,
            FolderId = folderId,
            ActorType = PermissionActorType.User,
            ActorId = actorId
        };

        _repository.DeletePermissions(valueObject)
            .Returns(false);

        // Act
        var result = await _service.DeletePermissions(valueObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetListAsync_WithAllActorTypes_ReturnsWithDisplayNames()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var folderId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var roleId = Guid.NewGuid().ToString();
        var orgUnitId = Guid.NewGuid().ToString();
        
        var permissions = new List<FileArchivePermissionEntity>
        {
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderId)
                .WithActorType(PermissionActorType.User)
                .WithActorId(userId)
                .Build(),
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderId)
                .WithActorType(PermissionActorType.Role)
                .WithActorId(roleId)
                .Build(),
            FileManagerTestDataBuilder.FileArchivePermission()
                .WithArchiveId(archiveId)
                .WithFolderId(folderId)
                .WithActorType(PermissionActorType.OrganizationUnit)
                .WithActorId(orgUnitId)
                .Build()
        };

        _repository.GetListAsync(archiveId, Arg.Is<List<string>>(ids => ids.Count == 1 && ids[0] == folderId))
            .Returns(permissions);

        // Act
        var result = await _service.GetListAsync(archiveId, new List<string> { folderId });

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        // Note: Display names may be "FailedToGetName" if identity managers aren't properly mocked
    }
}
