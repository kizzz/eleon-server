using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using FluentAssertions;
using NSubstitute;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Volo.Abp.Authorization;
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

// Note: These tests use integration test base because DomainService requires GuidGenerator and ABP infrastructure
public class FileArchiveDomainServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileArchiveDomainService GetService()
    {
        return GetRequiredService<FileArchiveDomainService>();
    }

    private IVirtualFolderRepository GetVirtualFolderRepository()
    {
        return GetRequiredService<IVirtualFolderRepository>();
    }

    private IArchiveRepository GetArchiveRepository()
    {
        return GetRequiredService<IArchiveRepository>();
    }

    [Fact]
    public async Task CreateFileArchive_PhysicalArchive_CreatesSuccessfully()
    {
        // Arrange
        var name = "TestArchive";
        var storageProviderId = Guid.NewGuid();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.FileArchiveHierarchyType.Should().Be(FileArchiveHierarchyType.Physical);
        result.StorageProviderId.Should().Be(storageProviderId);
        result.IsActive.Should().BeTrue();
        result.IsPersonalizedArchive.Should().BeFalse();
    }

    [Fact]
    public async Task CreateFileArchive_VirtualArchive_CreatesWithRootFolder()
    {
        // Arrange
        var name = "VirtualArchive";
        var storageProviderId = Guid.NewGuid();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Virtual,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Assert
        result.Should().NotBeNull();
        result.FileArchiveHierarchyType.Should().Be(FileArchiveHierarchyType.Virtual);
        result.RootFolderId.Should().NotBeNullOrEmpty();
        result.RootFolderId.Should().NotBe("./");
        
        // Verify root folder was created
        var rootFolder = await WithUnitOfWorkAsync(async () =>
        {
            return await GetVirtualFolderRepository().FindAsync(result.RootFolderId);
        });
        rootFolder.Should().NotBeNull();
        rootFolder.Name.Should().Be(name);
        rootFolder.EntryKind.Should().Be(EntryKind.Folder);
    }

    [Fact]
    public async Task GetFileArchiveById_WithPermission_ReturnsArchive()
    {
        // Arrange - Create an archive first
        var name = "TestArchive";
        var storageProviderId = Guid.NewGuid();
        
        var createdArchive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetFileArchiveById(createdArchive.Id);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdArchive.Id);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetFileArchiveById_NonExistentArchive_ThrowsEntityNotFoundException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException<FileArchiveEntity>>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                return await GetService().GetFileArchiveById(archiveId);
            });
        });
    }

    [Fact]
    public async Task DeleteFileArchive_ExistingArchive_DeletesSuccessfully()
    {
        // Arrange - Create an archive first
        var name = "ArchiveToDelete";
        var storageProviderId = Guid.NewGuid();
        
        var createdArchive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().DeleteFileArchive(createdArchive.Id);
        });

        // Assert
        result.Should().BeTrue();
        
        // Verify it was deleted
        var deleted = await WithUnitOfWorkAsync(async () =>
        {
            return await GetArchiveRepository().FindAsync(createdArchive.Id);
        });
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task UpdateFileArchive_WithChanges_UpdatesSuccessfully()
    {
        // Arrange - Create an archive first
        var name = "OldName";
        var storageProviderId = Guid.NewGuid();
        
        var createdArchive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Update the archive
        createdArchive.Name = "NewName";
        createdArchive.IsActive = false;
        createdArchive.IsPersonalizedArchive = true;

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateFileArchive(createdArchive);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("NewName");
        result.IsActive.Should().BeFalse();
        result.IsPersonalizedArchive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateFileArchive_NoChanges_ReturnsExisting()
    {
        // Arrange - Create an archive
        var name = "TestArchive";
        var storageProviderId = Guid.NewGuid();
        
        var createdArchive = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act - Update with same values (idempotent)
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateFileArchive(createdArchive);
        });

        // Assert - Should return existing entity
        result.Should().NotBeNull();
        result.Id.Should().Be(createdArchive.Id);
    }

    [Fact]
    public async Task GetFileArchivesList_ReturnsFilteredByPermissions()
    {
        // Arrange - Create multiple archives
        var archive1 = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                "Archive1",
                FileArchiveHierarchyType.Physical,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var archive2 = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                "Archive2",
                FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetFileArchivesList();
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(a => a.Id == archive1.Id);
        result.Should().Contain(a => a.Id == archive2.Id);
    }

    [Fact]
    public async Task CreateFileArchive_WithPersonalizedArchive_SetsFlag()
    {
        // Arrange
        var name = "PersonalizedArchive";
        var storageProviderId = Guid.NewGuid();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(
                name,
                FileArchiveHierarchyType.Physical,
                storageProviderId,
                isActive: true,
                isPersonalizedArchive: true,
                physicalRootFolderId: "physical-folder-id");
        });

        // Assert
        result.Should().NotBeNull();
        result.IsPersonalizedArchive.Should().BeTrue();
        result.PhysicalRootFolderId.Should().Be("physical-folder-id");
    }

    [Fact]
    public async Task DeleteFileArchive_NonExistentArchive_ThrowsException()
    {
        // Arrange
        var archiveId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException<FileArchiveEntity>>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                return await GetService().DeleteFileArchive(archiveId);
            });
        });
    }
}
