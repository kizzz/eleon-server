using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Storage.Module.Integration.Tests.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.S3BlobProviders;
using SharedModule.modules.Blob.Module.VfsShared;
using Xunit;

namespace Eleon.Storage.Module.Integration.Tests.VfsBlobProviders;

public class FileSystemVfsBlobProviderIntegrationTests : StorageModuleTestBase
{
    private FileSystemVfsBlobProvider _provider;
    private const string ContainerName = "default";

    public FileSystemVfsBlobProviderIntegrationTests()
    {
        var settings = StorageTestHelpers.BuildFileSystemSettings(TempDirectory);
        var settingsList = new List<StorageProviderSettingDto>();
        foreach (var kvp in settings)
        {
            settingsList.Add(new StorageProviderSettingDto
            {
                Key = kvp.Key,
                Value = kvp.Value,
                Id = Guid.NewGuid(),
                StorageProviderId = Guid.NewGuid()
            });
        }
        _provider = new FileSystemVfsBlobProvider(settingsList, SharedTestConstants.TenantIds.Tenant1);
    }

    [Fact]
    public async Task SaveAsync_Should_CreateFile()
    {
        // Arrange
        var blobName = "test-file.txt";
        var content = Encoding.UTF8.GetBytes("test content");
        var args = new VfsSaveArgs(ContainerName, blobName, new MemoryStream(content), overrideExisting: true);

        // Act
        await _provider.SaveAsync(args);

        // Assert
        var existsArgs = new VfsExistArgs(ContainerName, blobName);
        var exists = await _provider.ExistsAsync(existsArgs);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrNullAsync_Should_ReturnCorrectContent()
    {
        // Arrange
        var blobName = "test-get.txt";
        var expectedContent = "test content for get";
        var contentBytes = Encoding.UTF8.GetBytes(expectedContent);
        var saveArgs = new VfsSaveArgs(ContainerName, blobName, new MemoryStream(contentBytes), overrideExisting: true);
        await _provider.SaveAsync(saveArgs);

        // Act
        var getArgs = new VfsGetArgs(ContainerName, blobName);
        using var stream = await _provider.GetOrNullAsync(getArgs);

        // Assert
        stream.Should().NotBeNull();
        var actualContent = Encoding.UTF8.GetString(await stream.GetAllBytesAsync());
        actualContent.Should().Be(expectedContent);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveFile()
    {
        // Arrange
        var blobName = "test-delete.txt";
        var saveArgs = new VfsSaveArgs(ContainerName, blobName, new MemoryStream(Encoding.UTF8.GetBytes("content")), overrideExisting: true);
        await _provider.SaveAsync(saveArgs);

        // Act
        var deleteArgs = new VfsDeleteArgs(ContainerName, blobName);
        var deleted = await _provider.DeleteAsync(deleteArgs);

        // Assert
        deleted.Should().BeTrue();
        var existsArgs = new VfsExistArgs(ContainerName, blobName);
        var exists = await _provider.ExistsAsync(existsArgs);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task SaveAsync_WithOverrideExistingTrue_Should_OverwriteFile()
    {
        // Arrange
        var blobName = "test-overwrite.txt";
        var originalContent = "original";
        var newContent = "overwritten";
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, blobName, new MemoryStream(Encoding.UTF8.GetBytes(originalContent)), overrideExisting: true));
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, blobName, new MemoryStream(Encoding.UTF8.GetBytes(newContent)), overrideExisting: true));

        // Act
        var getArgs = new VfsGetArgs(ContainerName, blobName);
        using var stream = await _provider.GetOrNullAsync(getArgs);

        // Assert
        var content = Encoding.UTF8.GetString(await stream.GetAllBytesAsync());
        content.Should().Be(newContent);
    }

    [Fact]
    public async Task SaveAsync_WithIsFolderTrue_Should_CreateDirectory()
    {
        // Arrange
        var folderName = "test-folder";

        // Act
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, folderName, Stream.Null, overrideExisting: true, isFolder: true));

        // Assert
        var existsArgs = new VfsExistArgs(ContainerName, folderName);
        var exists = await _provider.ExistsAsync(existsArgs);
        exists.Should().BeTrue(); // May need adjustment based on actual behavior
    }

    [Fact]
    public async Task ListAsync_WithEmptyFolder_Should_ReturnEmptyList()
    {
        // Arrange
        var emptyFolder = "empty-folder";
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, emptyFolder, Stream.Null, overrideExisting: true, isFolder: true));

        // Act
        var listArgs = new VfsListArgs(ContainerName, emptyFolder, isRecursiveSearch: false);
        var result = await _provider.ListAsync(listArgs);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListAsync_WithFiles_Should_ReturnFiles()
    {
        // Arrange
        var folder = "list-folder";
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, $"{folder}/file1.txt", new MemoryStream(Encoding.UTF8.GetBytes("content1")), overrideExisting: true));
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, $"{folder}/file2.txt", new MemoryStream(Encoding.UTF8.GetBytes("content2")), overrideExisting: true));

        // Act
        var listArgs = new VfsListArgs(ContainerName, folder, isRecursiveSearch: false);
        var result = await _provider.ListAsync(listArgs);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Key.Contains("file1.txt"));
        result.Should().Contain(f => f.Key.Contains("file2.txt"));
    }

    [Fact]
    public async Task ListAsync_WithRecursiveSearch_Should_ReturnNestedFiles()
    {
        // Arrange
        var folder = "recursive-folder";
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, $"{folder}/sub/file.txt", new MemoryStream(Encoding.UTF8.GetBytes("content")), overrideExisting: true));

        // Act
        var listArgs = new VfsListArgs(ContainerName, folder, isRecursiveSearch: true);
        var result = await _provider.ListAsync(listArgs);

        // Assert
        result.Should().Contain(f => f.Key.Contains("file.txt"));
    }

    [Fact]
    public async Task ListPagedAsync_Should_ReturnPagedResults()
    {
        // Arrange
        var folder = "paged-folder";
        for (int i = 1; i <= 25; i++)
        {
            await _provider.SaveAsync(new VfsSaveArgs(ContainerName, $"{folder}/file{i}.txt", new MemoryStream(Encoding.UTF8.GetBytes($"content{i}")), overrideExisting: true));
        }

        // Act
        var listArgs = new VfsListPagedArgs(ContainerName, folder, isRecursiveSearch: false)
        {
            SkipResults = 0,
            MaxResults = 10
        };
        var result = await _provider.ListPagedAsync(listArgs);

        // Assert
        result.Items.Count.Should().BeLessThanOrEqualTo(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(25);
    }

    [Fact]
    public async Task SaveAsync_WithUnicodeFilename_Should_PreserveUnicode()
    {
        // Arrange
        var blobName = "тест-文件-テスト.txt";
        var content = Encoding.UTF8.GetBytes("unicode content");

        // Act
        await _provider.SaveAsync(new VfsSaveArgs(ContainerName, blobName, new MemoryStream(content), overrideExisting: true));

        // Assert
        var existsArgs = new VfsExistArgs(ContainerName, blobName);
        var exists = await _provider.ExistsAsync(existsArgs);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WithConcurrentSaves_Should_AllSucceed()
    {
        // Arrange
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 50; i++)
        {
            var blobName = $"concurrent-{i}.txt";
            var content = Encoding.UTF8.GetBytes($"content-{i}");
            tasks.Add(_provider.SaveAsync(new VfsSaveArgs(ContainerName, blobName, new MemoryStream(content), overrideExisting: true)));
        }
        await Task.WhenAll(tasks);

        // Assert
        var listArgs = new VfsListArgs(ContainerName, "concurrent-", isRecursiveSearch: false);
        var result = await _provider.ListAsync(listArgs);
        result.Count.Should().BeGreaterThanOrEqualTo(50);
    }

    public override void Dispose()
    {
        _provider?.Dispose();
        base.Dispose();
    }
}
