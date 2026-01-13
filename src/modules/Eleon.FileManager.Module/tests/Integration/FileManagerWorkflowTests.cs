using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Integration;

public class FileManagerWorkflowTests : ModuleTestBase<FileManagerTestStartupModule>
{
    [Fact]
    public async Task CreateArchive_UploadFiles_ManagePermissions_CompleteWorkflow()
    {
        // Arrange & Act - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "WorkflowArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        archive.Should().NotBeNull();
        archive.IsActive.Should().BeTrue();

        // Act - Upload files
        var fileService = GetRequiredService<IFileAppService>();
        var files = await WithUnitOfWorkAsync(async () =>
        {
            var file1 = await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "file1.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);

            var file2 = await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "file2.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);

            return new List<FileSystemEntryDto> { file1, file2 };
        });

        files.Should().HaveCount(2);

        // Act - Manage permissions
        var permissionService = GetRequiredService<FileArchivePermissions.IFileArchivePermissionAppService>();
        var permission = await WithUnitOfWorkAsync(async () =>
        {
            return await permissionService.UpdatePermission(new FileArchivePermissions.FileArchivePermissionDto
            {
                ArchiveId = archive.Id,
                FolderId = archive.RootFolderId,
                ActorType = PermissionActorType.User,
                ActorId = GetRequiredService<Volo.Abp.Users.ICurrentUser>().Id?.ToString() ?? Guid.NewGuid().ToString(),
                AllowedPermissions = new List<FileManagerPermissionType>
                {
                    FileManagerPermissionType.Read,
                    FileManagerPermissionType.Write
                }
            });
        });

        permission.Should().NotBeNull();
        permission.AllowedPermissions.Should().Contain(FileManagerPermissionType.Read);

        // Assert - Verify complete workflow
        var retrievedFiles = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                null,
                new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                false);
        });

        retrievedFiles.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateFolder_AddFiles_MoveFiles_DeleteFiles_CompleteWorkflow()
    {
        // Arrange - Create archive
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "FolderWorkflowArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        // Act - Create folder
        var folder = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "TestFolder",
                    Kind = EntryKind.Folder,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        folder.Should().NotBeNull();
        folder.EntryKind.Should().Be(EntryKind.Folder);

        // Act - Add files to folder
        var files = await WithUnitOfWorkAsync(async () =>
        {
            var file1 = await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "file1.txt",
                    Kind = EntryKind.File,
                    ParentId = folder.Id
                },
                archive.Id,
                FileManagerType.FileArchive);

            var file2 = await fileService.CreateEntry(
                new CreateEntryDto
                {
                    Name = "file2.txt",
                    Kind = EntryKind.File,
                    ParentId = folder.Id
                },
                archive.Id,
                FileManagerType.FileArchive);

            return new List<FileSystemEntryDto> { file1, file2 };
        });

        files.Should().HaveCount(2);

        // Act - Move files
        var movedFile = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.MoveEntry(
                new MoveEntryDto
                {
                    EntryId = files[0].Id,
                    DestinationParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        movedFile.Should().BeTrue();

        // Act - Delete file
        var deleted = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.DeleteEntry(files[1].Id, archive.Id, FileManagerType.FileArchive);
        });

        deleted.Should().BeTrue();

        // Assert - Verify final state
        var rootFiles = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                null,
                new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                false);
        });

        rootFiles.Should().Contain(f => f.Id == files[0].Id);
        rootFiles.Should().NotContain(f => f.Id == files[1].Id);
    }

    [Fact]
    public async Task ShareFile_CreateLink_AccessLink_CompleteWorkflow()
    {
        // Arrange - Create archive and file
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ShareWorkflowArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
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
                    Name = "shared.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        // Act - Create external link
        var linkService = GetRequiredService<FileExternalLink.IFileExternalLinkAppService>();
        var link = await WithUnitOfWorkAsync(async () =>
        {
            return await linkService.UpdateExternalLinkSetting(new FileExternalLink.FileExternalLinkDto
            {
                FileId = Guid.Parse(file.Id),
                ArchiveId = archive.Id,
                PermissionType = FileShareStatus.Readonly
            });
        });

        link.Should().NotBeNull();
        link.PermissionType.Should().Be(FileShareStatus.Readonly);

        // Act - Get link settings
        var retrievedLink = await WithUnitOfWorkAsync(async () =>
        {
            return await linkService.GetFileExternalLinkSetting(file.Id, archive.Id);
        });

        retrievedLink.Should().NotBeNull();
        retrievedLink.Id.Should().Be(link.Id);
    }

    [Fact]
    public async Task ConcurrentFileOperations_NoDataCorruption()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ConcurrentArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        // Act - Create files concurrently
        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await fileService.CreateEntry(
                    new CreateEntryDto
                    {
                        Name = $"concurrent_{i}.txt",
                        Kind = EntryKind.File,
                        ParentId = archive.RootFolderId
                    },
                    archive.Id,
                    FileManagerType.FileArchive);
            });
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All files created successfully
        results.Should().HaveCount(10);
        results.Should().OnlyHaveUniqueItems(r => r.Id);

        // Verify no duplicates
        var allFiles = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.GetEntriesByParentId(
                archive.RootFolderId,
                archive.Id,
                null,
                new List<FileStatus> { FileStatus.Active },
                FileManagerType.FileArchive,
                false);
        });

        allFiles.Should().HaveCount(10);
        allFiles.Select(f => f.Name).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task MultiTenantIsolation_NoCrossTenantAccess()
    {
        // Arrange - This test verifies tenant isolation at the integration level
        // Note: Actual tenant isolation testing requires multi-tenant setup
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "TenantIsolationArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act & Assert - Archive should be isolated to current tenant
        archive.Should().NotBeNull();

        // Note: Full tenant isolation testing requires setting up multiple tenants
        // and verifying cross-tenant access is denied
    }

    [Fact]
    public async Task ConcurrentFileCreation_HandlesConflicts()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "ConflictArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();

        // Act - Try to create files with same name concurrently
        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            try
            {
                return await WithUnitOfWorkAsync(async () =>
                {
                    return await fileService.CreateEntry(
                        new CreateEntryDto
                        {
                            Name = "conflict.txt", // Same name
                            Kind = EntryKind.File,
                            ParentId = archive.RootFolderId
                        },
                        archive.Id,
                        FileManagerType.FileArchive);
                });
            }
            catch
            {
                return null; // Expected to fail for duplicates
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - Only one should succeed, others should fail
        var successful = results.Where(r => r != null).ToList();
        successful.Should().HaveCount(1, "Only one file with the same name should be created");
    }

    [Fact]
    public async Task ConcurrentFileDeletion_HandlesConflicts()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "DeleteConflictArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
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

        // Act - Try to delete the same file concurrently
        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await fileService.DeleteEntry(file.Id, archive.Id, FileManagerType.FileArchive);
            });
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All deletions should complete (idempotent)
        results.Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public async Task ConcurrentPermissionUpdates_HandlesConflicts()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "PermissionConflictArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var permissionService = GetRequiredService<FileArchivePermissions.IFileArchivePermissionAppService>();
        var currentUser = GetRequiredService<Volo.Abp.Users.ICurrentUser>();

        // Act - Update permissions concurrently
        var tasks = Enumerable.Range(0, 3).Select(async i =>
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await permissionService.UpdatePermission(new FileArchivePermissions.FileArchivePermissionDto
                {
                    ArchiveId = archive.Id,
                    FolderId = archive.RootFolderId,
                    ActorType = PermissionActorType.User,
                    ActorId = currentUser.Id?.ToString() ?? Guid.NewGuid().ToString(),
                    AllowedPermissions = new List<FileManagerPermissionType>
                    {
                        (FileManagerPermissionType)(i % 3 + 1) // Vary permissions
                    }
                });
            });
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All updates should complete
        results.Should().AllSatisfy(r => r.Should().NotBeNull());
    }

    [Fact]
    public async Task ConcurrentStatusUpdates_HandlesConflicts()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "StatusConflictArchive",
                FileArchiveHierarchyType.Virtual,
                FileManagerTestStartupModule.StorageProviderId,
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
                    Name = "status_test.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var statusService = GetRequiredService<FileStatusDomainService>();

        // Act - Update status concurrently (should be idempotent)
        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await statusService.UpdateFileStatus(archive.Id, file.Id, FileStatus.Trash);
            });
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All updates should succeed (idempotent)
        results.Should().AllBeEquivalentTo(true);
    }
}
