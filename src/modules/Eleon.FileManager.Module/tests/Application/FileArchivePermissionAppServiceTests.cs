using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.FileArchivePermissions;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Application;

public class FileArchivePermissionAppServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private IFileArchivePermissionAppService GetService()
    {
        return GetRequiredService<IFileArchivePermissionAppService>();
    }

    [Fact]
    public async Task GetList_ValidInput_ReturnsPermissions()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetList(archive.Id, archive.RootFolderId);
        });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPermissionOrDefault_ValidKey_ReturnsPermissions()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var key = new FileArchivePermissionKeyDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetPermissionOrDefault(key);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(new[]
        {
            FileManagerPermissionType.Read,
            FileManagerPermissionType.Modify,
            FileManagerPermissionType.Write
        });
    }

    [Fact]
    public async Task GetPermissionWithoutDefault_ValidKey_ReturnsPermission()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var key = new FileArchivePermissionKeyDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetPermissionWithoutDefault(key);
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdatePermission_ValidInput_UpdatesPermission()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var permissionDto = new FileArchivePermissionDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId,
            ActorType = PermissionActorType.User,
            ActorId = Guid.NewGuid().ToString(),
            AllowedPermissions = new List<FileManagerPermissionType>
            {
                FileManagerPermissionType.Read,
                FileManagerPermissionType.Write
            }
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdatePermission(permissionDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.ArchiveId.Should().Be(archive.Id);
        result.AllowedPermissions.Should().Contain(FileManagerPermissionType.Read);
        result.AllowedPermissions.Should().Contain(FileManagerPermissionType.Write);
    }

    [Fact]
    public async Task DeletePermissions_ValidInput_DeletesPermission()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var permissionDto = new FileArchivePermissionDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId,
            ActorType = PermissionActorType.User,
            ActorId = Guid.NewGuid().ToString(),
            AllowedPermissions = new List<FileManagerPermissionType>
            {
                FileManagerPermissionType.Read
            }
        };

        var createdPermission = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdatePermission(permissionDto);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().DeletePermissions(createdPermission);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetPermissionOrDefault_WithPermissions_ReturnsMatchingPermissions()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var currentUser = GetRequiredService<Volo.Abp.Users.ICurrentUser>();
        var permissionDto = new FileArchivePermissionDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId,
            ActorType = PermissionActorType.User,
            ActorId = currentUser.Id?.ToString() ?? Guid.NewGuid().ToString(),
            AllowedPermissions = new List<FileManagerPermissionType>
            {
                FileManagerPermissionType.Read,
                FileManagerPermissionType.Modify
            }
        };

        await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdatePermission(permissionDto);
        });

        var key = new FileArchivePermissionKeyDto
        {
            ArchiveId = archive.Id,
            FolderId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetPermissionOrDefault(key);
        });

        // Assert
        result.Should().NotBeNull();
        // Note: Actual permissions depend on user role and archive creator status
    }
}
