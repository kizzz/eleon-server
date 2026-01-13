using System;
using System.Threading.Tasks;
using FluentAssertions;
using VPortal.FileManager.Module.Controllers;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.HttpApi.Controllers;

public class FileExternalLinkControllerTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileExternalLinkController GetController()
    {
        return new FileExternalLinkController(
            GetRequiredService<Logging.Module.IVportalLogger<FileExternalLinkController>>(),
            GetRequiredService<FileExternalLink.IFileExternalLinkAppService>());
    }

    [Fact]
    public async Task CreateLink_ValidInput_ReturnsCreated()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "LinkArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "link_test.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var controller = GetController();
        var linkDto = new FileExternalLink.FileExternalLinkDto
        {
            FileId = Guid.Parse(file.Id),
            ArchiveId = archive.Id,
            PermissionType = Common.Module.Constants.FileShareStatus.Readonly
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.UpdateExternalLinkSetting(linkDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().Be(file.Id);
    }

    [Fact]
    public async Task GetLink_ValidId_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "LinkArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "link_test.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkService = GetRequiredService<FileExternalLink.IFileExternalLinkAppService>();
        var link = await WithUnitOfWorkAsync(async () =>
        {
            return await linkService.UpdateExternalLinkSetting(new FileExternalLink.FileExternalLinkDto
            {
                FileId = Guid.Parse(file.Id),
                ArchiveId = archive.Id,
                PermissionType = Common.Module.Constants.FileShareStatus.Readonly
            });
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.GetFileExternalLinkSetting(file.Id, archive.Id);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(link.Id);
    }

    [Fact]
    public async Task UpdateLink_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "LinkArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "link_test.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkService = GetRequiredService<FileExternalLink.IFileExternalLinkAppService>();
        var link = await WithUnitOfWorkAsync(async () =>
        {
            return await linkService.UpdateExternalLinkSetting(new FileExternalLink.FileExternalLinkDto
            {
                FileId = Guid.Parse(file.Id),
                ArchiveId = archive.Id,
                PermissionType = Common.Module.Constants.FileShareStatus.Readonly
            });
        });

        link.PermissionType = Common.Module.Constants.FileShareStatus.Modify;

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.UpdateExternalLinkSetting(link);
        });

        // Assert
        result.Should().NotBeNull();
        result.PermissionType.Should().Be(Common.Module.Constants.FileShareStatus.Readonly);
    }

    [Fact]
    public async Task DeleteLink_ValidId_ReturnsNoContent()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "LinkArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "link_test.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkService = GetRequiredService<FileExternalLink.IFileExternalLinkAppService>();
        var link = await WithUnitOfWorkAsync(async () =>
        {
            return await linkService.UpdateExternalLinkSetting(new FileExternalLink.FileExternalLinkDto
            {
                FileId = Guid.Parse(file.Id),
                ArchiveId = archive.Id,
                PermissionType = Common.Module.Constants.FileShareStatus.Readonly
            });
        });

        var controller = GetController();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                return await controller.DeleteExternalLinkSetting(link.Id);
            });
        });
    }
}
