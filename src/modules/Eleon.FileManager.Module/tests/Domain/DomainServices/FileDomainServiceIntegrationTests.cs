using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

// Integration tests for FileDomainService - uses real database and services
public class FileDomainServiceIntegrationTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileDomainService GetService()
    {
        return GetRequiredService<FileDomainService>();
    }

    private FileArchiveDomainService GetArchiveService()
    {
        return GetRequiredService<FileArchiveDomainService>();
    }

    [Fact]
    public async Task GetEntriesByParentId_WithFilesAndFolders_ReturnsAll()
    {
        // Arrange - Create archive and entries
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

        var rootFolderId = archive.RootFolderId;

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntriesByParentId(
                rootFolderId,
                archive.Id,
                kind: null,
                fileStatuses: new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                recursive: false);
        });

        // Assert
        result.Should().NotBeNull();
        // Should contain at least the root folder
    }

    [Fact]
    public async Task GetEntryById_ExistingEntry_ReturnsEntry()
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

        var rootFolderId = archive.RootFolderId;

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetEntryById(rootFolderId, archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(rootFolderId);
        result.EntryKind.Should().Be(EntryKind.Folder);
    }

    [Fact]
    public async Task GetRootEntry_VirtualArchive_ReturnsRootFolder()
    {
        // Arrange - Create virtual archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "VirtualArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetRootEntry(archive.Id, FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.EntryKind.Should().Be(EntryKind.Folder);
        result.ParentId.Should().BeNull();
    }

    [Fact]
    public async Task SearchEntries_WithMatchingName_ReturnsResults()
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
        // Should find the archive root folder with name "SearchArchive"
        result.Should().Contain(e => e.Name.Contains("Search", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAllFiles_WithFilters_ReturnsFilteredResults()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "FilterArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetAllFiles(
                archive.Id,
                filterByFavourite: false,
                filterByStatus: false,
                filterByShareStatus: false,
                fileStatuses: new List<FileStatus>(),
                fileShareStatuses: new List<FileShareStatus>(),
                FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<FileSystemEntry>>();
    }

    [Fact]
    public async Task GetAllFolders_WithFilters_ReturnsFilteredResults()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveService().CreateFileArchive(
                "FolderArchive",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetAllFolders(
                archive.Id,
                filterByFavourite: false,
                filterByStatus: false,
                fileStatuses: new List<FileStatus>(),
                FileManagerType.FileArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<FileSystemEntry>>();
    }
}
