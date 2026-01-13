using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.Controllers;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.FileArchivePermissions;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.HttpApi.Controllers;

public class FileArchivePermissionControllerTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileArchivePermissionController GetController()
    {
        return new FileArchivePermissionController(
            GetRequiredService<Logging.Module.IVportalLogger<FileArchivePermissionController>>(),
            GetRequiredService<IFileArchivePermissionAppService>());
    }

    [Fact]
    public async Task GrantPermission_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "PermissionArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();
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
            return await controller.UpdatePermission(permissionDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.AllowedPermissions.Should().Contain(FileManagerPermissionType.Read);
    }

    [Fact]
    public async Task RevokePermission_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "PermissionArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var permissionService = GetRequiredService<IFileArchivePermissionAppService>();
        var permission = await WithUnitOfWorkAsync(async () =>
        {
            return await permissionService.UpdatePermission(new FileArchivePermissionDto
            {
                ArchiveId = archive.Id,
                FolderId = archive.RootFolderId,
                ActorType = PermissionActorType.User,
                ActorId = Guid.NewGuid().ToString(),
                AllowedPermissions = new List<FileManagerPermissionType>
                {
                    FileManagerPermissionType.Read
                }
            });
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.DeletePermissions(permission);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetPermissions_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "PermissionArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.GetList(archive.Id, archive.RootFolderId);
        });

        // Assert
        result.Should().NotBeNull();
    }
}
