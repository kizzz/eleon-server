using System;
using System.IO;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Commons.Module.Messages.Google;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using FluentAssertions;
using NSubstitute;
using Storage.Module.BlobProviders.GoogleDrive;
using Volo.Abp.BlobStoring;
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace Eleon.Storage.Module.Contract.Tests.CustomBlobProviders;

public class GoogleDriveBlobProviderContractTests : MockingTestBase
{
    private GoogleDriveBlobProvider _provider;
    private IResponseCapableEventBus _responseBus;
    private IDistributedEventBus _eventBus;

    public GoogleDriveBlobProviderContractTests()
    {
        _responseBus = CreateMockResponseCapableEventBus();
        _eventBus = (IDistributedEventBus)_responseBus;
        _provider = new GoogleDriveBlobProvider(_eventBus);
    }

    [Fact]
    public async Task SaveAsync_Should_EmitUploadAndCreateLinkMessages()
    {
        // Arrange
        var blobName = "test-file.txt";
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var fileId = "drive-file-id-123";
        var webViewLink = "https://drive.google.com/file/view/123";
        
        var uploadResponse = new GoogleDriveUploadResponseMsg { FileId = fileId };
        var linkResponse = new GoogleDriveCreateLinkResponseMsg { WebViewLink = webViewLink };
        
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveUploadRequestMsg, GoogleDriveUploadResponseMsg>(
            _responseBus, uploadResponse);
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveCreateLinkRequestMsg, GoogleDriveCreateLinkResponseMsg>(
            _responseBus, linkResponse);

        var args = new BlobProviderSaveArgs("default", new BlobContainerConfiguration(), blobName, new MemoryStream(content), false);

        // Act
        await _provider.SaveAsync(args);

        // Assert
        await _responseBus.Received(1).RequestAsync<GoogleDriveUploadResponseMsg>(
            Arg.Is<GoogleDriveUploadRequestMsg>(msg => msg.Name == blobName));
        await _responseBus.Received(1).RequestAsync<GoogleDriveCreateLinkResponseMsg>(
            Arg.Is<GoogleDriveCreateLinkRequestMsg>(msg => msg.FileId == fileId));
        await _eventBus.Received(1).PublishAsync(
            Arg.Is<GoogleDriveBlobSavedEvent>(evt => 
                evt.BlobName == blobName && 
                evt.FileId == fileId &&
                evt.WebUrl == webViewLink));
    }

    [Fact]
    public async Task SaveAsync_WithUploadError_Should_ThrowIOException()
    {
        // Arrange
        var uploadResponse = new GoogleDriveUploadResponseMsg { FileId = null, Error = "Upload failed" };
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveUploadRequestMsg, GoogleDriveUploadResponseMsg>(
            _responseBus, uploadResponse);

        var args = new BlobProviderSaveArgs("default", new BlobContainerConfiguration(), "test.txt", new MemoryStream(new byte[] { 1 }), false);

        // Act & Assert
        await _provider.Invoking(p => p.SaveAsync(args))
            .Should().ThrowAsync<IOException>()
            .WithMessage("*Upload failed*");
    }

    [Fact]
    public async Task SaveAsync_WithMissingFileId_Should_ThrowIOException()
    {
        // Arrange
        var uploadResponse = new GoogleDriveUploadResponseMsg { FileId = string.Empty };
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveUploadRequestMsg, GoogleDriveUploadResponseMsg>(
            _responseBus, uploadResponse);

        var args = new BlobProviderSaveArgs("default", new BlobContainerConfiguration(), "test.txt", new MemoryStream(new byte[] { 1 }), false);

        // Act & Assert
        await _provider.Invoking(p => p.SaveAsync(args))
            .Should().ThrowAsync<IOException>()
            .WithMessage("*missing file id*");
    }

    [Fact]
    public async Task DeleteAsync_Should_EmitDeleteMessage()
    {
        // Arrange
        var fileId = "drive-file-id";
        var deleteResponse = new GoogleDriveDeleteResponseMsg { Success = true };
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveDeleteRequestMsg, GoogleDriveDeleteResponseMsg>(
            _responseBus, deleteResponse);

        var args = new BlobProviderDeleteArgs("default", new BlobContainerConfiguration(), fileId);

        // Act
        var result = await _provider.DeleteAsync(args);

        // Assert
        result.Should().BeTrue();
        await _responseBus.Received(1).RequestAsync<GoogleDriveDeleteResponseMsg>(
            Arg.Is<GoogleDriveDeleteRequestMsg>(msg => msg.FileId == fileId));
    }

    [Fact]
    public async Task DeleteAsync_WithError_Should_ThrowIOException()
    {
        // Arrange
        var deleteResponse = new GoogleDriveDeleteResponseMsg { Success = false, Error = "Delete failed" };
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveDeleteRequestMsg, GoogleDriveDeleteResponseMsg>(
            _responseBus, deleteResponse);

        var args = new BlobProviderDeleteArgs("default", new BlobContainerConfiguration(), "file-id");

        // Act & Assert
        await _provider.Invoking(p => p.DeleteAsync(args))
            .Should().ThrowAsync<IOException>()
            .WithMessage("*Delete failed*");
    }

    [Fact]
    public async Task GetOrNullAsync_Should_EmitDownloadMessage()
    {
        // Arrange
        var fileId = "drive-file-id";
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var downloadResponse = new GoogleDriveDownloadResponseMsg { Content = content };
        EventBusTestHelpers.SetupEventBusRequestAsync<GoogleDriveDownloadRequestMsg, GoogleDriveDownloadResponseMsg>(
            _responseBus, downloadResponse);

        var args = new BlobProviderGetArgs("default", new BlobContainerConfiguration(), fileId);

        // Act
        using var stream = await _provider.GetOrNullAsync(args);

        // Assert
        stream.Should().NotBeNull();
        var resultBytes = await stream.GetAllBytesAsync();
        resultBytes.Should().BeEquivalentTo(content);
        await _responseBus.Received(1).RequestAsync<GoogleDriveDownloadResponseMsg>(
            Arg.Is<GoogleDriveDownloadRequestMsg>(msg => msg.FileId == fileId));
    }

    [Fact]
    public void ExistsAsync_Should_ReturnTrue()
    {
        // Arrange
        var args = new BlobProviderExistsArgs("default", new BlobContainerConfiguration(), "file-id");

        // Act
        var result = _provider.ExistsAsync(args).Result;

        // Assert
        result.Should().BeTrue(); // Currently always returns true
    }
}
