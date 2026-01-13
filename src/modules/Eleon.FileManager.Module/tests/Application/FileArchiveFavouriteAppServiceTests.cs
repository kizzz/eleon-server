using System;
using System.Threading.Tasks;
using FluentAssertions;
using VPortal.FileManager.Module.FileArchiveFavourites;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Application;

// Integration tests for FileArchiveFavouriteAppService
public class FileArchiveFavouriteAppServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private IFileArchiveFavouriteAppService GetService()
    {
        return GetRequiredService<IFileArchiveFavouriteAppService>();
    }

    [Fact]
    public async Task AddToFavourites_ValidInput_AddsSuccessfully()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var dto = new FileArchiveFavouriteDto
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().AddToFavourites(dto);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveFromFavourites_ExistingFavourite_RemovesSuccessfully()
    {
        // Arrange - Add favourite first
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var addDto = new FileArchiveFavouriteDto
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        await WithUnitOfWorkAsync(async () =>
        {
            await GetService().AddToFavourites(addDto);
        });

        var removeDto = new FileArchiveFavouriteDto
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().RemoveFromFavourites(removeDto);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveFromFavourites_NonExistentFavourite_ReturnsTrue()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();

        var dto = new FileArchiveFavouriteDto
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().RemoveFromFavourites(dto);
        });

        // Assert - Should return true even if favourite doesn't exist
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddToFavourites_DuplicateFavourite_StillCreates()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var fileId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        var dto = new FileArchiveFavouriteDto
        {
            ArchiveId = archiveId,
            FileId = fileId,
            FolderId = folderId,
            ParentId = parentId
        };

        // Add first time
        await WithUnitOfWorkAsync(async () =>
        {
            await GetService().AddToFavourites(dto);
        });

        // Act - Add again
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().AddToFavourites(dto);
        });

        // Assert - Should still return true (creates duplicate)
        result.Should().BeTrue();
    }
}
