using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Application;

// Integration tests for FileAppService
public class FileAppServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private IFileAppService GetService()
    {
        return GetRequiredService<IFileAppService>();
    }

    private FileArchiveDomainService GetArchiveService()
    {
        return GetRequiredService<FileArchiveDomainService>();
    }

    [Fact]
    public async Task GetEntriesByParentId_WithArchive_ReturnsEntries()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "TestArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                kind: null,
                fileStatuses: new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                recursive: false);
        });

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEntryById_ExistingEntry_ReturnsEntry()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "EntryArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntryById(archive.RootFolderId, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(archive.RootFolderId);
    }

    [Fact]
    public async Task GetEntriesByParentIdPaged_WithPaging_ReturnsPagedResults()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "PagedArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var input = new GetFileEntriesByParentPagedInput
        {
            FolderId = archive.RootFolderId,
            SkipCount = 0,
            MaxResultCount = 10,
            Sorting = "Name"
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntriesByParentIdPaged(
                input,
                archive.Id,
                kind: null,
                fileStatuses: new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                recursive: false);
        });

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetEntriesByIds_WithIds_ReturnsEntries()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "IdsArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var ids = new List<string> { archive.RootFolderId };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntriesByIds(ids, archive.Id, kind: null, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(e => e.Id == archive.RootFolderId);
    }

    [Fact]
    public async Task SearchEntries_WithQuery_ReturnsMatchingEntries()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "SearchArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().SearchEntries("Search", archive.Id, kind: null, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(e => e.Name.Contains("Search", StringComparison.OrdinalIgnoreCase));
    }
}
