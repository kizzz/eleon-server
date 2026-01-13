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
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace Eleon.Storage.Module.Contract.Tests.CustomBlobProviders;

/// <summary>
/// Contract tests for VfsStorageProviderCacheManager focusing on event bus message contracts.
/// </summary>
public class VfsStorageProviderCacheManagerContractTests : MockingTestBase
{
    private IResponseCapableEventBus _eventBus;
    private ILogger<VfsStorageProviderCacheManager> _logger;
    private VfsBlobProviderCacheService _cacheService;

    public VfsStorageProviderCacheManagerContractTests()
    {
        _eventBus = CreateMockResponseCapableEventBus();
        _logger = Substitute.For<ILogger<VfsStorageProviderCacheManager>>();
        _cacheService = new VfsBlobProviderCacheService();
    }

    [Fact]
    public async Task ResolveProviderAsync_Should_EmitCorrectGetStorageProviderMsg()
    {
        // Arrange
        var manager = new EleonsoftSdk.modules.StorageProvider.Module.VfsStorageProviderCacheManager(
            _logger,
            (IDistributedEventBus)_eventBus,
            _cacheService);
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerId = "provider-id-123";
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test"),
            Guid.NewGuid());
        provider.IsActive = true;

        var response = StorageTestHelpers.BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            _eventBus, response);

        // Act
        await manager.ResolveProviderAsync(tenantId, providerId);

        // Assert - Verify correct message contract
        await _eventBus.Received(1).RequestAsync<GetStorageProviderResponseMsg>(
            Arg.Is<GetStorageProviderMsg>(msg => 
                msg.StorageProviderId == providerId));
    }

    [Fact]
    public async Task ResolveProviderAsync_WithTelemetryProviderKey_Should_CacheTelemetryMapping()
    {
        // Arrange
        var manager = new EleonsoftSdk.modules.StorageProvider.Module.VfsStorageProviderCacheManager(
            _logger,
            (IDistributedEventBus)_eventBus,
            _cacheService);
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
        await manager.ResolveProviderAsync(tenantId, BlobMessagingConsts.TelemetryStorageProviderKey);

        // Assert - Verify telemetry mapping cached
        var cachedId = manager.GetCachedTelemetryProviderId(tenantId);
        cachedId.Should().Be(realProviderId.ToString());
    }
}
