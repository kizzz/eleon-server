using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Commons.Module.Messages.Features;
using Common.EventBus.Module;
using FluentAssertions;
using NSubstitute;
using SharedModule.modules.Blob.Module.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.BlobStoring;
using Volo.Abp.MultiTenancy;
using VPortal.FileManager.Module.Managers;
using VPortal.FileManager.Module.Tests.TestBase;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;
using Xunit;

namespace VPortal.FileManager.Module.Tests.Domain.Managers;

public class FileEditManagerTests : DomainTestBase
{
    private readonly FileEditManager _manager;
    private readonly StorageDomainService _storageDomainService;
    private readonly StorageProviderOptionsManager _storageProviderOptionsManager;
    private readonly IBlobContainer _blobContainer;
    private readonly IBlobContainerFactory _blobContainerFactory;
    private readonly ContainersCacheDomainService _containersCache;
    private readonly IResponseCapableEventBus _eventBus;

    public FileEditManagerTests()
    {
        _eventBus = CreateMockResponseCapableEventBus();
        SetupEventBusRequestAsync<GetFeatureSettingMsg, GetFeatureSettingResponseMsg>(
            _eventBus,
            new GetFeatureSettingResponseMsg { Value = string.Empty });

        _storageProviderOptionsManager = new StorageProviderOptionsManager(
            CreateMockObjectMapper(),
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)_eventBus,
            CreateMockCurrentTenant());

        _blobContainer = Substitute.For<IBlobContainer>();
        _blobContainerFactory = Substitute.For<IBlobContainerFactory>();
        _blobContainerFactory.Create(Arg.Any<string>()).Returns(_blobContainer);
        _containersCache = new ContainersCacheDomainService(CreateMockLogger<ContainersCacheDomainService>());

        _storageDomainService = new StorageDomainService(
            CreateMockLogger<StorageDomainService>(),
            _blobContainerFactory,
            _containersCache,
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)_eventBus,
            _storageProviderOptionsManager);
        SetCurrentTenant(_storageDomainService, CreateMockCurrentTenant());

        _manager = new FileEditManager(
            CreateMockLogger<FileEditManager>(),
            _storageDomainService,
            _storageProviderOptionsManager);
    }

    private static void SetCurrentTenant(Volo.Abp.Domain.Services.DomainService service, ICurrentTenant currentTenant)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>().Returns(currentTenant);

        var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
            "LazyServiceProvider",
            BindingFlags.Public | BindingFlags.Instance);
        lazyServiceProviderProp?.SetValue(service, lazyServiceProvider);
    }

    #region Upload Tests

    [Fact]
    public async Task Upload_ValidInput_ReturnsNulls()
    {
        // Arrange
        var fileName = "test.txt";
        var fileData = new byte[] { 1, 2, 3, 4, 5 };
        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(fileName, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _manager.Upload(fileName, fileData);

        // Assert
        result.Should().NotBeNull();
        result.Item1.Should().BeNull();
        result.Item2.Should().BeNull();
    }

    [Fact]
    public async Task Upload_EmptyData_CompletesWithoutError()
    {
        // Arrange
        var fileName = "empty.txt";
        var fileData = Array.Empty<byte>();

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(fileName, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _manager.Upload(fileName, fileData);

        // Assert
        result.Should().NotBeNull();
        result.Item1.Should().BeNull();
        result.Item2.Should().BeNull();
    }

    [Fact]
    public async Task Upload_StorageServiceThrowsException_ReturnsNull()
    {
        // Arrange
        var fileName = "test.txt";
        var fileData = new byte[] { 1, 2, 3 };

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(fileName, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Storage error")));

        // Act
        var result = await _manager.Upload(fileName, fileData);

        // Assert - Exception is caught and logged, returns (null, null)
        result.Should().NotBeNull();
        result.Item1.Should().BeNull();
        result.Item2.Should().BeNull();
    }

    [Fact]
    public async Task Upload_NullFileName_CompletesWithoutError()
    {
        // Arrange
        var fileData = new byte[] { 1, 2, 3 };

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(null, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _manager.Upload(null, fileData);

        // Assert
        result.Should().NotBeNull();
        result.Item1.Should().BeNull();
        result.Item2.Should().BeNull();
    }

    [Fact]
    public async Task Upload_NullData_CompletesWithoutError()
    {
        // Arrange
        var fileName = "test.txt";

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(fileName, Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _manager.Upload(fileName, null);

        // Assert
        result.Should().NotBeNull();
        result.Item1.Should().BeNull();
        result.Item2.Should().BeNull();
    }

    #endregion

    #region DeleteFile Tests

    [Fact]
    public async Task DeleteFile_ValidId_DeletesFile()
    {
        // Arrange
        var externalFileId = "external-123";

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.DeleteAsync(externalFileId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        await _manager.DeleteFile(externalFileId);

        // Assert
        await _blobContainer.Received(1).DeleteAsync(externalFileId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteFile_NonExistentId_CompletesWithoutError()
    {
        // Arrange
        var externalFileId = "non-existent-123";

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.DeleteAsync(externalFileId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("File not found")));

        // Act - Exception is caught and logged, method completes
        await _manager.DeleteFile(externalFileId);

        // Assert - No exception propagated
    }

    [Fact]
    public async Task DeleteFile_NullId_CompletesWithoutError()
    {
        // Arrange
        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.DeleteAsync(null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        await _manager.DeleteFile(null);

        // Assert - No exception thrown
    }

    [Fact]
    public async Task DeleteFile_EmptyId_CompletesWithoutError()
    {
        // Arrange
        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.DeleteAsync(string.Empty, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        await _manager.DeleteFile("");

        // Assert - No exception thrown
    }

    #endregion

    #region DownloadFile Tests

    [Fact]
    public async Task DownloadFile_ValidId_ReturnsFileData()
    {
        // Arrange
        var externalFileId = "external-123";
        var expectedData = new byte[] { 1, 2, 3, 4, 5 };

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.GetOrNullAsync(externalFileId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(new MemoryStream(expectedData)));

        // Act
        var result = await _manager.DownloadFile(externalFileId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task DownloadFile_NonExistentId_ReturnsNull()
    {
        // Arrange
        var externalFileId = "non-existent-123";

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.GetOrNullAsync(externalFileId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(null));

        // Act
        var result = await _manager.DownloadFile(externalFileId);

        // Assert - Exception is caught and logged, returns null
        result.Should().BeNull();
    }

    [Fact]
    public async Task DownloadFile_EmptyId_ReturnsNull()
    {
        // Arrange
        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.GetOrNullAsync(string.Empty, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(null));

        // Act
        var result = await _manager.DownloadFile("");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DownloadFile_NullId_ReturnsNull()
    {
        // Arrange
        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.GetOrNullAsync(null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(null));

        // Act
        var result = await _manager.DownloadFile(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DownloadFile_LargeFile_ReturnsAllData()
    {
        // Arrange
        var externalFileId = "external-123";
        var largeData = new byte[10 * 1024 * 1024]; // 10 MB
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.GetOrNullAsync(externalFileId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(new MemoryStream(largeData)));

        // Act
        var result = await _manager.DownloadFile(externalFileId);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(largeData.Length);
        result.Should().BeEquivalentTo(largeData);
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public async Task Upload_ConcurrentUploads_HandlesCorrectly()
    {
        // Arrange
        var fileName1 = "file1.txt";
        var fileName2 = "file2.txt";
        var data1 = new byte[] { 1, 2, 3 };
        var data2 = new byte[] { 4, 5, 6 };

        var settingsGroup = _storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        _blobContainer.SaveAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var task1 = _manager.Upload(fileName1, data1);
        var task2 = _manager.Upload(fileName2, data2);
        await Task.WhenAll(task1, task2);

        // Assert
        task1.Result.Should().NotBeNull();
        task2.Result.Should().NotBeNull();
    }

    #endregion
}
