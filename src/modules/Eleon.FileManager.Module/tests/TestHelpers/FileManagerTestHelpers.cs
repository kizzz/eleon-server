using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using NSubstitute;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.Tests.TestHelpers;

/// <summary>
/// Test helpers for file management scenarios that can be reused within FileManager module tests.
/// </summary>
public static class FileManagerTestHelpers
{
    /// <summary>
    /// Creates a mock file repository with standard setup.
    /// </summary>
    /// <returns>A mock IFileSystemEntryRepository.</returns>
    public static IFileSystemEntryRepository CreateMockFileRepository()
    {
        return Substitute.For<IFileSystemEntryRepository>();
    }

    /// <summary>
    /// Creates a mock archive repository with standard setup.
    /// </summary>
    /// <returns>A mock IArchiveRepository.</returns>
    public static IArchiveRepository CreateMockArchiveRepository()
    {
        return Substitute.For<IArchiveRepository>();
    }

    /// <summary>
    /// Sets up a file hierarchy (nested folders and files) for testing.
    /// </summary>
    /// <param name="repository">The repository to configure.</param>
    /// <param name="rootFolderId">The root folder ID.</param>
    /// <param name="depth">The depth of nesting (default: 2).</param>
    /// <param name="filesPerFolder">Number of files per folder (default: 2).</param>
    /// <returns>Dictionary mapping folder IDs to their children.</returns>
    public static Dictionary<string, List<FileSystemEntry>> SetupFileHierarchy(
        IFileSystemEntryRepository repository,
        string rootFolderId,
        int depth = 2,
        int filesPerFolder = 2)
    {
        var hierarchy = new Dictionary<string, List<FileSystemEntry>>();
        var allEntries = new List<FileSystemEntry>();

        // Create root folder
        var rootFolder = FileSystemEntry.CreateFolder(
            rootFolderId,
            Guid.NewGuid(),
            "Root",
            null,
            null,
            false,
            null);

        allEntries.Add(rootFolder);
        hierarchy[rootFolderId] = new List<FileSystemEntry>();

        // Create nested structure
        var folderStack = new Stack<(string FolderId, int CurrentDepth)>();
        folderStack.Push((rootFolderId, 0));

        while (folderStack.Count > 0)
        {
            var (parentId, currentDepth) = folderStack.Pop();

            if (currentDepth >= depth)
                continue;

            // Create subfolders
            for (int i = 0; i < 2; i++)
            {
                var folderId = Guid.NewGuid().ToString();
                var folder = FileSystemEntry.CreateFolder(
                    folderId,
                    Guid.NewGuid(),
                    $"Folder_{currentDepth}_{i}",
                    parentId,
                    null,
                    false,
                    null);

                allEntries.Add(folder);
                if (!hierarchy.ContainsKey(parentId))
                    hierarchy[parentId] = new List<FileSystemEntry>();
                hierarchy[parentId].Add(folder);
                hierarchy[folderId] = new List<FileSystemEntry>();

                if (currentDepth + 1 < depth)
                    folderStack.Push((folderId, currentDepth + 1));
            }

            // Create files in this folder
            for (int i = 0; i < filesPerFolder; i++)
            {
                var fileId = Guid.NewGuid().ToString();
                var file = FileSystemEntry.CreateFile(
                    fileId,
                    Guid.NewGuid(),
                    $"File_{currentDepth}_{i}.txt",
                    parentId,
                    ".txt",
                    $"/path/to/{fileId}",
                    "1024",
                    null);

                allEntries.Add(file);
                if (!hierarchy.ContainsKey(parentId))
                    hierarchy[parentId] = new List<FileSystemEntry>();
                hierarchy[parentId].Add(file);
            }
        }

        // Setup repository to return entries by parent
        foreach (var kvp in hierarchy)
        {
            repository.GetEntriesByParentId(kvp.Key, null, false)
                .Returns(kvp.Value);
        }

        // Setup GetEntryById for all entries
        foreach (var entry in allEntries)
        {
            repository.GetEntryById(entry.Id).Returns(entry);
        }

        return hierarchy;
    }

    /// <summary>
    /// Verifies that file permission checks are performed correctly.
    /// </summary>
    /// <param name="hasPermission">Whether permission check should return true.</param>
    /// <param name="permissionType">The permission type to check.</param>
    /// <param name="actualResult">The actual permission check result.</param>
    public static void VerifyFilePermissions(
        bool hasPermission,
        FileManagerPermissionType permissionType,
        bool actualResult)
    {
        actualResult.Should().Be(hasPermission,
            $"Permission check for {permissionType} should return {hasPermission}");
    }

    /// <summary>
    /// Creates a scenario for testing concurrent file operations.
    /// </summary>
    /// <param name="operation">The file operation to execute concurrently.</param>
    /// <param name="concurrencyLevel">Number of concurrent operations.</param>
    /// <returns>Results from concurrent operations.</returns>
    public static async Task<List<T>> CreateConcurrentFileOperation<T>(
        Func<Task<T>> operation,
        int concurrencyLevel = 10)
    {
        return await ConcurrencyTestHelpers.SimulateConcurrentOperationAsync(operation, concurrencyLevel);
    }

    /// <summary>
    /// Sets up a file factory mock to return the specified repository for a given archive.
    /// </summary>
    /// <param name="factory">The file factory to configure.</param>
    /// <param name="archive">The archive entity.</param>
    /// <param name="fileManagerType">The file manager type.</param>
    /// <param name="repository">The repository to return.</param>
    public static void SetupFileFactory(
        IFileFactory factory,
        FileArchiveEntity archive,
        FileManagerType fileManagerType,
        IFileSystemEntryRepository repository)
    {
        factory.Get(archive, fileManagerType).Returns(repository);
    }

    /// <summary>
    /// Creates a test archive entity with standard configuration.
    /// </summary>
    /// <param name="archiveId">The archive ID (optional).</param>
    /// <param name="name">The archive name (optional).</param>
    /// <param name="isActive">Whether the archive is active (default: true).</param>
    /// <returns>A FileArchiveEntity.</returns>
    public static FileArchiveEntity CreateTestArchive(
        Guid? archiveId = null,
        string name = "TestArchive",
        bool isActive = true)
    {
        var id = archiveId ?? Guid.NewGuid();
        return new FileArchiveEntity(id)
        {
            Name = name,
            FileArchiveHierarchyType = FileArchiveHierarchyType.Virtual,
            StorageProviderId = Guid.NewGuid(),
            FileEditStorageProviderId = Guid.NewGuid(),
            RootFolderId = Guid.NewGuid().ToString(),
            PhysicalRootFolderId = Guid.NewGuid().ToString(),
            IsActive = isActive,
            IsPersonalizedArchive = false
        };
    }
}
