using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using VPortal.FileManager.Module.FileArchives;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.FileManager.Module.Tests.TestHelpers;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Application;

// Integration tests for FileArchiveAppService
public class FileArchiveAppServiceTests : ModuleTestBase<FileManagerTestStartupModule>
{
    private IFileArchiveAppService GetService()
    {
        return GetRequiredService<IFileArchiveAppService>();
    }

    [Fact]
    public async Task CreateFileArchive_ValidInput_CreatesSuccessfully()
    {
        // Arrange
        var dto = new FileArchiveDto
        {
            Name = "TestArchive",
            FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
            StorageProviderId = Guid.NewGuid(),
            IsActive = true,
            IsPersonalizedArchive = false
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(dto);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.FileArchiveHierarchyType.Should().Be(dto.FileArchiveHierarchyType);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetFileArchiveById_ExistingArchive_ReturnsArchive()
    {
        // Arrange - Create archive first
        var dto = new FileArchiveDto
        {
            Name = "GetArchive",
            FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
            StorageProviderId = Guid.NewGuid(),
            IsActive = true
        };

        var created = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(dto);
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetFileArchiveById(created.Id);
        });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(created.Id);
        result.Name.Should().Be(dto.Name);
    }

    [Fact]
    public async Task GetFileArchivesList_ReturnsAllArchives()
    {
        // Arrange - Create multiple archives
        var archive1 = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(new FileArchiveDto
            {
                Name = "Archive1",
                FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
                StorageProviderId = Guid.NewGuid(),
                IsActive = true
            });
        });

        var archive2 = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(new FileArchiveDto
            {
                Name = "Archive2",
                FileArchiveHierarchyType = FileArchiveHierarchyType.Virtual,
                StorageProviderId = Guid.NewGuid(),
                IsActive = true
            });
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
    public async Task UpdateFileArchive_WithChanges_UpdatesSuccessfully()
    {
        // Arrange - Create archive
        var created = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(new FileArchiveDto
            {
                Name = "OldName",
                FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
                StorageProviderId = Guid.NewGuid(),
                IsActive = true
            });
        });

        created.Name = "NewName";
        created.IsActive = false;

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().UpdateFileArchive(created);
        });

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("NewName");
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteFileArchive_ExistingArchive_DeletesSuccessfully()
    {
        // Arrange - Create archive
        var created = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(new FileArchiveDto
            {
                Name = "DeleteArchive",
                FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
                StorageProviderId = Guid.NewGuid(),
                IsActive = true
            });
        });

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().DeleteFileArchive(created.Id);
        });

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetFileArchivesListByParams_WithPaging_ReturnsPagedResults()
    {
        // Arrange
        var input = new FileArchiveListRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10,
            Sorting = "Name"
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().GetFileArchivesListByParams(input);
        });

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task CreateFileArchive_VirtualArchive_CreatesWithRootFolder()
    {
        // Arrange
        var dto = new FileArchiveDto
        {
            Name = "VirtualArchive",
            FileArchiveHierarchyType = FileArchiveHierarchyType.Virtual,
            StorageProviderId = Guid.NewGuid(),
            IsActive = true
        };

        // Act
        var result = await WithUnitOfWorkAsync(async () =>
        {
            return await GetService().CreateFileArchive(dto);
        });

        // Assert
        result.Should().NotBeNull();
        result.FileArchiveHierarchyType.Should().Be(FileArchiveHierarchyType.Virtual);
        result.RootFolderId.Should().NotBeNullOrEmpty();
    }
}
