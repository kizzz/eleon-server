using System;
using System.Threading.Tasks;
using FluentAssertions;
using VPortal.FileManager.Module.Controllers;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Tests.TestBase;
using Xunit;

namespace VPortal.FileManager.Module.Tests.HttpApi.Controllers;

public class FileArchiveFavouriteControllerTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private FileArchiveFavouriteController GetController()
    {
        return new FileArchiveFavouriteController(
            GetRequiredService<Logging.Module.IVportalLogger<FileArchiveFavouriteController>>(),
            GetRequiredService<FileArchiveFavourites.IFileArchiveFavouriteAppService>());
    }

    [Fact]
    public async Task AddToFavourites_ValidInput_ReturnsCreated()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "FavouriteArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "favourite.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var controller = GetController();
        var favouriteDto = new FileArchiveFavourites.FileArchiveFavouriteDto
        {
            ArchiveId = archive.Id,
            FileId = file.Id,
            FolderId = archive.RootFolderId,
            ParentId = archive.RootFolderId
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.AddToFavourites(favouriteDto);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveFromFavourites_ValidInput_ReturnsNoContent()
    {
        // Arrange
        var archive = await WithUnitOfWorkAsync(async () =>
        {
            var archiveService = GetRequiredService<FileArchiveDomainService>();
            return await archiveService.CreateFileArchive(
                "FavouriteArchive",
                Common.Module.Constants.FileArchiveHierarchyType.Virtual,
                Guid.NewGuid(),
                isActive: true,
                isPersonalizedArchive: false,
                physicalRootFolderId: null);
        });

        var fileService = GetRequiredService<IFileAppService>();
        var file = await WithUnitOfWorkAsync(async () =>
        {
            return await fileService.CreateEntry(
                new Files.CreateEntryDto
                {
                    Name = "favourite.txt",
                    Kind = EntryKind.File,
                    ParentId = archive.RootFolderId
                },
                archive.Id,
                FileManagerType.FileArchive);
        });

        var favouriteService = GetRequiredService<FileArchiveFavourites.IFileArchiveFavouriteAppService>();
        var favouriteDto = new FileArchiveFavourites.FileArchiveFavouriteDto
        {
            ArchiveId = archive.Id,
            FileId = file.Id,
            FolderId = archive.RootFolderId,
            ParentId = archive.RootFolderId
        };

        await WithUnitOfWorkAsync(async () =>
        {
            return await favouriteService.AddToFavourites(favouriteDto);
        });

        var controller = GetController();

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await controller.RemoveFromFavourites(favouriteDto);
        });

        // Assert
        result.Should().BeTrue();
    }

    // Note: no GetFavourites endpoint exists on controller; covered by app service tests instead.
}
