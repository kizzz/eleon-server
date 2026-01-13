using System;
using System.Threading.Tasks;
using FluentAssertions;
using VPortal.FileManager.Module.Controllers;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.FileArchives;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.HttpApi.Controllers;

public class FileArchiveControllerTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileArchiveController GetController()
    {
        return new FileArchiveController(
            GetRequiredService<Logging.Module.IVportalLogger<FileArchiveController>>(),
            GetRequiredService<IFileArchiveAppService>());
    }

    [Fact]
    public async Task GetArchive_ValidId_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
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
            return await controller.GetFileArchiveById(archive.Id);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(archive.Id);
    }

    [Fact]
    public async Task CreateArchive_ValidInput_ReturnsCreated()
    {
        // Arrange
        var controller = GetController();
        var archiveDto = new FileArchiveDto
        {
            Name = "NewArchive",
            FileArchiveHierarchyType = Common.Module.Constants.FileArchiveHierarchyType.Virtual,
            StorageProviderId = Guid.NewGuid(),
            IsActive = true,
            IsPersonalizedArchive = false
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.CreateFileArchive(archiveDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("NewArchive");
    }

    [Fact]
    public async Task UpdateArchive_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var appService = GetRequiredService<IFileArchiveAppService>();
        var archiveDto = await WithUnitOfWorkAsync(async () =>
        {
            return await appService.GetFileArchiveById(archive.Id);
        });

        archiveDto.Name = "UpdatedArchive";

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.UpdateFileArchive(archiveDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("UpdatedArchive");
    }

    [Fact]
    public async Task DeleteArchive_ValidId_ReturnsNoContent()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "DeleteArchive",
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
            return await controller.DeleteFileArchive(archive.Id);
        });

        // Assert
        result.Should().BeTrue();
    }
}
