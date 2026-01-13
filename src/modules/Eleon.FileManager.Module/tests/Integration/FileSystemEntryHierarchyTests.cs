using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Integration;

public class FileSystemEntryHierarchyTests : ModuleTestBase<FileManagerTestStartupModule>
{
    [Fact]
    public async Task CreateNestedFolders_ValidHierarchy()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "HierarchyArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        // Act - Create nested folder structure
        var level1 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Level1",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var level2 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Level2",
                    Kind = EntryKind.Folder,
                    ParentId = level1.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var level3 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Level3",
                    Kind = EntryKind.Folder,
                    ParentId = level2.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Assert
        level1.Should().NotBeNull();
        level2.Should().NotBeNull();
        level2.ParentId.Should().Be(level1.Id);
        level3.Should().NotBeNull();
        level3.ParentId.Should().Be(level2.Id);

        // Verify hierarchy
        var parents = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntryParentsById(level3.Id, archive.Id, FileManagerType.FileArchive);
        });

        parents.Should().NotBeNull();
        parents.Should().Contain(p => p.Id == level1.Id);
        parents.Should().Contain(p => p.Id == level2.Id);
    }

    [Fact]
    public async Task MoveFolder_UpdatesAllChildren()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "MoveFolderArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        var sourceFolder = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "SourceFolder",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var childFile = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "child.txt",
                    Kind = EntryKind.File,
                    ParentId = sourceFolder.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var destinationFolder = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "DestinationFolder",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act - Move source folder to destination
        var moved = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.MoveEntry(
                new MoveEntryDto
                {
                    EntryId = sourceFolder.Id,
                    DestinationParentId = destinationFolder.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        moved.Should().BeTrue();

        // Assert - Verify folder moved
        var movedFolder = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntryById(sourceFolder.Id, archive.Id, FileManagerType.FileArchive);
        });

        movedFolder.Should().NotBeNull();
        movedFolder.ParentId.Should().Be(destinationFolder.Id);

        // Verify child file still has correct parent
        var updatedChild = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntryById(childFile.Id, archive.Id, FileManagerType.FileArchive);
        });

        updatedChild.Should().NotBeNull();
        updatedChild.ParentId.Should().Be(sourceFolder.Id);
    }

    [Fact]
    public async Task DeleteFolder_DeletesAllChildren()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "DeleteFolderArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        var folder = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "FolderToDelete",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var childFile = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "child.txt",
                    Kind = EntryKind.File,
                    ParentId = folder.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act - Delete folder
        var deleted = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.DeleteEntry(folder.Id, archive.Id, FileManagerType.FileArchive);
        });

        deleted.Should().BeTrue();

        // Assert - Folder and child should be deleted
        var deletedFolder = await WithUnitOfWorkAsync(async () =>
        {
            try
            {
                return await fileService.GetEntryById(folder.Id, archive.Id, FileManagerType.FileArchive);
            }
            catch
            {
                return null;
            }
        });

        deletedFolder.Should().NotBeNull();

        var deletedChild = await WithUnitOfWorkAsync(async () =>
        {
            try
            {
                return await fileService.GetEntryById(childFile.Id, archive.Id, FileManagerType.FileArchive);
            }
            catch
            {
                return null;
            }
        });

        deletedChild.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHierarchy_ReturnsCompleteTree()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TreeArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        var folder1 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Folder1",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var folder2 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Folder2",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var file1 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "file1.txt",
                    Kind = EntryKind.File,
                    ParentId = folder1.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act - Get hierarchy recursively
        var hierarchy = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                null,
                new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                recursive: true);
        });

        // Assert
        hierarchy.Should().NotBeNull();
        hierarchy.Should().Contain(e => e.Id == folder1.Id);
        hierarchy.Should().Contain(e => e.Id == folder2.Id);
        hierarchy.Should().Contain(e => e.Id == file1.Id);
    }

    [Fact]
    public async Task CircularReference_Prevented()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "CircularArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        var folder1 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Folder1",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var folder2 = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "Folder2",
                    Kind = EntryKind.Folder,
                    ParentId = folder1.Id
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act & Assert - Try to move folder1 under folder2 (would create cycle)
        await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                return await fileService.MoveEntry(
                    new MoveEntryDto
                    {
                        EntryId = folder1.Id,
                        DestinationParentId = folder2.Id
                    },
                    archive.Id,
                    FileManagerType.FileArchive);
            });
        });
    }

    [Fact]
    public async Task DeepNesting_HandledCorrectly()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "DeepNestingArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        // Act - Create deep nesting (10 levels)
        var folders = new List<FileSystemEntryDto>();
        string currentParentId = archive.RootFolderId;

        for (int i = 0; i < 10; i++)
        {
            var folder = await WithUnitOfWorkAsync(async () =>
            {
                return await fileService.CreateEntry(
                    new CreateEntryDto
                    {
                        Name = $"Level{i}",
                        Kind = EntryKind.Folder,
                        ParentId = currentParentId
                    },
                    archive.Id,
                    FileManagerType.FileArchive);
            });

            folders.Add(folder);
            currentParentId = folder.Id;
        }

        // Assert - All folders created
        folders.Should().HaveCount(10);

        // Verify deepest folder
        var deepestFolder = folders.Last();
        var parents = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntryParentsById(deepestFolder.Id, archive.Id, FileManagerType.FileArchive);
        });

        parents.Should().NotBeNull();
        parents.Should().HaveCount(11);
    }
}
