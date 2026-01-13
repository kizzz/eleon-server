using System;
using Common.EventBus.Module;
using Commons.Module.Messages.Features;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using NSubstitute;
using SharedModule.modules.Blob.Module.Models;
using VPortal.Storage.Module.DynamicOptions;
using Volo.Abp.BlobStoring;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Eleon.Storage.Module.Domain.Tests.TestBase;

/// <summary>
/// Base class for Storage Module domain unit tests.
/// Provides mocked dependencies and helpers for testing domain logic.
/// </summary>
public abstract class StorageDomainTestBase : MockingTestBase
{
    protected IResponseCapableEventBus MockEventBus { get; }
    protected IBlobContainerFactory MockBlobContainerFactory { get; }
    protected ICurrentTenant MockCurrentTenant { get; }
    protected IObjectMapper MockObjectMapper { get; }

    protected StorageDomainTestBase()
    {
        MockEventBus = CreateMockResponseCapableEventBus();
        MockBlobContainerFactory = Substitute.For<IBlobContainerFactory>();
        MockCurrentTenant = CreateMockCurrentTenant();
        MockObjectMapper = CreateMockObjectMapper();
    }

    /// <summary>
    /// Sets up event bus to return a storage provider response.
    /// </summary>
    protected void SetupStorageProviderResponse(StorageProviderDto provider)
    {
        var response = StorageTestHelpers.BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            MockEventBus, response);
    }

    /// <summary>
    /// Sets up event bus to return a feature setting value.
    /// </summary>
    protected void SetupFeatureSettingResponse(string value)
    {
        var response = new GetFeatureSettingResponseMsg { Value = value };
        EventBusTestHelpers.SetupEventBusRequestAsync<GetFeatureSettingMsg, GetFeatureSettingResponseMsg>(
            MockEventBus, response);
    }

    /// <summary>
    /// Sets up event bus to return a feature setting based on request.
    /// </summary>
    protected void SetupFeatureSettingResponse(Func<GetFeatureSettingMsg, GetFeatureSettingResponseMsg> responseFactory)
    {
        EventBusTestHelpers.SetupEventBusRequestAsync<GetFeatureSettingMsg, GetFeatureSettingResponseMsg>(
            MockEventBus, responseFactory);
    }

    /// <summary>
    /// Creates a StorageProviderOptionsManager instance with mocked dependencies.
    /// </summary>
    protected StorageProviderOptionsManager CreateStorageProviderOptionsManager(
        IResponseCapableEventBus eventBus = null,
        ICurrentTenant currentTenant = null,
        IObjectMapper objectMapper = null)
    {
        var responseBus = eventBus ?? MockEventBus;
        return new StorageProviderOptionsManager(
            objectMapper ?? MockObjectMapper,
            (IDistributedEventBus)responseBus,
            currentTenant ?? MockCurrentTenant);
    }
}
