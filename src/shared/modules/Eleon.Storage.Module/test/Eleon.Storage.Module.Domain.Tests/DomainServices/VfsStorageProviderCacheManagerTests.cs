using System;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.Storage.Lib.Constants;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace Eleon.Storage.Module.Domain.Tests.DomainServices;

public class VfsStorageProviderCacheManagerTests : MockingTestBase
{
    private VfsStorageProviderCacheManager _manager;
    private IResponseCapableEventBus _eventBus;
    private ILogger<VfsStorageProviderCacheManager> _logger;
    private VfsBlobProviderCacheService _cacheService;

    public VfsStorageProviderCacheManagerTests()
    {
        _eventBus = CreateMockResponseCapableEventBus();
        _logger = Substitute.For<ILogger<VfsStorageProviderCacheManager>>();
        _cacheService = new VfsBlobProviderCacheService();
        _manager = new VfsStorageProviderCacheManager(_logger, (IDistributedEventBus)_eventBus, _cacheService);
    }

    [Fact]
    public void TryGetCachedProvider_WithCachedProvider_Should_ReturnTrueAndProvider()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();
        var cachedProvider = Substitute.For<IVfsBlobProvider>();
        _cacheService.Register($"{tenantId};{providerId}", cachedProvider);

        // Act
        var result = _manager.TryGetCachedProvider(tenantId, providerId, out var provider);

        // Assert
        result.Should().BeTrue();
        provider.Should().BeSameAs(cachedProvider);
        _eventBus.DidNotReceive().RequestAsync<GetStorageProviderResponseMsg>(Arg.Any<object>());
    }

    [Fact]
    public void TryGetCachedProvider_WithMissingCache_Should_ReturnFalse()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();

        // Act
        var result = _manager.TryGetCachedProvider(tenantId, providerId, out var provider);

        // Assert
        result.Should().BeFalse();
        provider.Should().BeNull();
    }

    [Fact]
    public void TryGetCachedProvider_WithNullProviderId_Should_ReturnFalse()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;

        // Act
        var result = _manager.TryGetCachedProvider(tenantId, null, out var provider);

        // Assert
        result.Should().BeFalse();
        provider.Should().BeNull();
    }

    [Fact]
    public async Task ResolveProviderAsync_WithCacheMiss_Should_CallEventBusAndCacheProvider()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test"),
            Guid.Parse(providerId));
        provider.IsActive = true;

        var response = StorageTestHelpers.BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            _eventBus, response);

        // Act
        var result = await _manager.ResolveProviderAsync(tenantId, providerId);

        // Assert
        result.Should().NotBeNull();
        _eventBus.Received(1).RequestAsync<GetStorageProviderResponseMsg>(
            Arg.Is<GetStorageProviderMsg>(msg => msg.StorageProviderId == providerId));
        
        // Verify cached
        _manager.TryGetCachedProvider(tenantId, providerId, out var cached).Should().BeTrue();
    }

    [Fact]
    public async Task ResolveProviderAsync_WithNullResponse_Should_Throw()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();
        var response = new GetStorageProviderResponseMsg { StorageProvider = null };
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            _eventBus, response);

        // Act & Assert
        await _manager.Invoking(m => m.ResolveProviderAsync(tenantId, providerId))
            .Should().ThrowAsync<Exception>()
            .WithMessage($"*Storage provider for tenant '{tenantId}' not found*");
    }

    [Fact]
    public async Task ResolveProviderAsync_WithTelemetryProviderKey_Should_CacheMapping()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var realProviderId = Guid.NewGuid();
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test"),
            realProviderId);
        provider.IsActive = true;

        var response = StorageTestHelpers.BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            _eventBus, response);

        // Act
        await _manager.ResolveProviderAsync(tenantId, BlobMessagingConsts.TelemetryStorageProviderKey);

        // Assert
        var cachedId = _manager.GetCachedTelemetryProviderId(tenantId);
        cachedId.Should().Be(realProviderId.ToString());
    }

    [Fact]
    public void GetCachedTelemetryProviderId_WhenNotCached_Should_ReturnDefaultKey()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant2;

        // Act
        var result = _manager.GetCachedTelemetryProviderId(tenantId);

        // Assert
        result.Should().Be(BlobMessagingConsts.TelemetryStorageProviderKey);
    }

    [Fact]
    public void UpdateBlobProviderCache_Should_RemoveFromCache()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();
        var provider = Substitute.For<IVfsBlobProvider>();
        _cacheService.Register($"{tenantId};{providerId}", provider);

        // Act
        _manager.UpdateBlobProviderCache(tenantId, providerId);

        // Assert
        _cacheService.TryGet($"{tenantId};{providerId}", out _).Should().BeFalse();
    }

    [Fact]
    public async Task ResolveProviderAsync_WithConcurrentCalls_Should_CallEventBusOnce()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = Guid.NewGuid().ToString();
        var realProviderId = Guid.Parse(providerId);
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test"),
            realProviderId);
        provider.IsActive = true;

        var response = StorageTestHelpers.BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            _eventBus, response);

        // Act
        var results = await ConcurrencyTestHelpers.SimulateConcurrentOperationAsync(
            () => _manager.ResolveProviderAsync(tenantId, providerId),
            concurrencyLevel: 10);

        // Assert
        _eventBus.Received(1).RequestAsync<GetStorageProviderResponseMsg>(Arg.Any<object>());
        results.Should().AllBeEquivalentTo(results[0]);
    }

    public void Dispose()
    {
        _manager?.Dispose();
        _cacheService?.Dispose();
    }
}
