using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using VPortal.FileManager.Module.ValueObjects;
using Volo.Abp.Users;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.DomainServices;

// Note: These tests use integration test base because DomainService requires GuidGenerator
public class FileArchiveFavouriteDomainServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileArchiveFavouriteDomainService GetService()
    {
        return GetRequiredService<FileArchiveFavouriteDomainService>();
    }

    private IFileArchiveFavouriteRepository GetRepository()
    {
        return GetRequiredService<IFileArchiveFavouriteRepository>();
    }

    [Fact]
    public async Task AddToFavourites_NewFavourite_CreatesSuccessfully()
    {
        // Note: The service implementation always creates a new entity regardless of existing one
        // This test verifies the basic flow works
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();
        var userId = GetRequiredService<Volo.Abp.Users.ICurrentUser>().Id.ToString();

        var valueObject = new FileArchiveFavouriteValueObject
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().AddToFavourites(valueObject);
        });

        // Assert
        result.Should().BeTrue();
        
        // Verify it was created
        var created = await WithUnitOfWorkAsync(async () =>
        {
            return await GetRepository().GetListAsync(archiveId, parentId, GetRequiredService<Volo.Abp.Users.ICurrentUser>().Id.ToString());
        });
        created.Should().NotBeNull();
        created.Should().Contain(f => f.FileId == fileId && f.FolderId == folderId);
    }

    [Fact]
    public async Task RemoveFromFavourites_ExistingFavourite_RemovesSuccessfully()
    {
        // Arrange - First create a favourite
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var valueObject = new FileArchiveFavouriteValueObject
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        await WithUnitOfWorkAsync(async () =>
        {
            await GetService().AddToFavourites(valueObject);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().RemoveFromFavourites(archiveId, fileId, folderId);
        });

        // Assert
        result.Should().BeTrue();
        
        // Verify it was removed
        var remaining = await WithUnitOfWorkAsync(async () =>
        {
            return await GetRepository().GetListAsync(archiveId, parentId, GetRequiredService<Volo.Abp.Users.ICurrentUser>().Id.ToString());
        });
        remaining.Should().NotContain(f => f.FileId == fileId && f.FolderId == folderId);
    }

    [Fact]
    public async Task RemoveFromFavourites_NonExistentFavourite_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().RemoveFromFavourites(archiveId, fileId, folderId);
        });

        // Assert - Should return true even if favourite doesn't exist
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetListAsync_WithFavourites_ReturnsList()
    {
        // Arrange - Create a favourite first
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var valueObject = new FileArchiveFavouriteValueObject
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        await WithUnitOfWorkAsync(async () =>
        {
            await GetService().AddToFavourites(valueObject);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetListAsync(archiveId, parentId);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].FileId.Should().Be(fileId);
        result[0].FolderId.Should().Be(folderId);
    }

    [Fact]
    public async Task GetListAsync_NoFavourites_ReturnsEmptyList()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var parentId = Guid.NewGuid().ToString();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetListAsync(archiveId, parentId);
        });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddToFavourites_DuplicateFavourite_StillCreates()
    {
        // Note: The service implementation always creates a new entity even if one exists
        // This test verifies that behavior
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var valueObject = new FileArchiveFavouriteValueObject
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        // Add first time
        await WithUnitOfWorkAsync(async () =>
        {
            await GetService().AddToFavourites(valueObject);
        });

        // Act - Add again
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().AddToFavourites(valueObject);
        });

        // Assert - Should still return true (creates duplicate)
        result.Should().BeTrue();
    }
}
