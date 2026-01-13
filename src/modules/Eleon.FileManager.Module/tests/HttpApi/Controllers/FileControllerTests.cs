using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Controllers;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.HttpApi.Controllers;

public class FileControllerTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileController GetController()
    {
        return new FileController(
            GetRequiredService<Logging.Module.IVportalLogger<FileController>>(),
            GetRequiredService<IFileAppService>());
    }

    [Fact]
    public async Task GetEntryById_ValidId_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.GetEntryById(archive.RootFolderId, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(archive.RootFolderId);
    }

    [Fact]
    public async Task GetEntryById_InvalidId_ReturnsNull()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();
        var invalidId = Guid.NewGuid().ToString();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            try
            {
                return await controller.GetEntryById(invalidId, archive.Id, FileManagerType.FileArchive);
            }
            catch
            {
                return null;
            }
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEntriesByParentId_ValidParent_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                null,
                new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                false);
        });

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateFile_ValidInput_ReturnsCreated()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();
        var createDto = new CreateEntryDto
        {
            Name = "controller_test.txt",
            Kind = EntryKind.File,
            ParentId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.CreateEntry(createDto, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("controller_test.txt");
        result.EntryKind.Should().Be(EntryKind.File);
    }

    [Fact]
    public async Task CreateFolder_ValidInput_ReturnsCreated()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var controller = GetController();
        var createDto = new CreateEntryDto
        {
            Name = "ControllerFolder",
            Kind = EntryKind.Folder,
            ParentId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.CreateEntry(createDto, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.EntryKind.Should().Be(EntryKind.Folder);
    }

    [Fact]
    public async Task UpdateFile_ValidInput_ReturnsOk()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "original.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var controller = GetController();
        var renameDto = new RenameEntryDto
        {
            Id = file.Id,
            Name = "renamed.txt"
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.RenameEntry(renameDto, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("renamed.txt");
    }

    [Fact]
    public async Task DeleteFile_ValidId_ReturnsNoContent()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "delete_me.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.DeleteEntry(file.Id, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SearchEntries_WithQuery_ReturnsMatchingEntries()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ControllerArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        await WithUnitOfWorkAsync(async () =>
        {
            await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "searchable.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.SearchEntries("searchable", archive.Id, null, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(e => e.Name.Contains("searchable", StringComparison.OrdinalIgnoreCase));
    }
}
