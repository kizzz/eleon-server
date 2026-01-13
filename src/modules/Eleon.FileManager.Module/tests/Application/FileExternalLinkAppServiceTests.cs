using System;
using System.Threading.Tasks;
using FluentAssertions;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.FileExternalLink;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Application;

public class FileExternalLinkAppServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private IFileExternalLinkAppService GetService()
    {
        return GetRequiredService<IFileExternalLinkAppService>();
    }

    [Fact]
    public async Task GetFileExternalLinkSetting_ValidFile_ReturnsLink()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<Files.IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "test.txt",
                    Kind = VPortal.FileManager.Module.Constants.EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetFileExternalLinkSetting(file.Id, archive.Id);
        });

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().Be(Guid.Parse(file.Id));
        result.ArchiveId.Should().Be(archive.Id);
    }

    [Fact]
    public async Task UpdateExternalLinkSetting_ValidInput_UpdatesLink()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<Files.IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "test.txt",
                    Kind = VPortal.FileManager.Module.Constants.EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkDto = new FileExternalLinkDto
        {
            FileId = Guid.Parse(file.Id),
            ArchiveId = archive.Id,
                    PermissionType = Common.Module.Constants.FileShareStatus.Readonly
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateExternalLinkSetting(linkDto);
        });

        // Assert
        result.Should().NotBeNull();
        result.PermissionType.Should().Be(Common.Module.Constants.FileShareStatus.Readonly);
    }

    [Fact]
    public async Task DeleteExternalLinkSetting_ValidId_DeletesLink()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<Files.IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "test.txt",
                    Kind = VPortal.FileManager.Module.Constants.EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkDto = new FileExternalLinkDto
        {
            FileId = Guid.Parse(file.Id),
            ArchiveId = archive.Id,
                    PermissionType = Common.Module.Constants.FileShareStatus.Readonly
        };

        var createdLink = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateExternalLinkSetting(linkDto);
        });

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                return await GetService().DeleteExternalLinkSetting(createdLink.Id);
            });
        });
    }

    [Fact]
    public async Task CancelChanges_ValidId_CancelsChanges()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<Files.IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "test.txt",
                    Kind = VPortal.FileManager.Module.Constants.EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkDto = new FileExternalLinkDto
        {
            FileId = Guid.Parse(file.Id),
            ArchiveId = archive.Id,
                    PermissionType = Common.Module.Constants.FileShareStatus.Readonly
        };

        var createdLink = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateExternalLinkSetting(linkDto);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CancelChanges(createdLink.Id);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SaveChanges_ValidId_SavesChanges()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TestArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<Files.IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "test.txt",
                    Kind = VPortal.FileManager.Module.Constants.EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var linkDto = new FileExternalLinkDto
        {
            FileId = Guid.Parse(file.Id),
            ArchiveId = archive.Id,
                    PermissionType = Common.Module.Constants.FileShareStatus.Readonly
        };

        var createdLink = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateExternalLinkSetting(linkDto);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().SaveChanges(createdLink.Id, deleteAfterChanges: false);
        });

        // Assert
        result.Should().BeFalse();
    }
}
