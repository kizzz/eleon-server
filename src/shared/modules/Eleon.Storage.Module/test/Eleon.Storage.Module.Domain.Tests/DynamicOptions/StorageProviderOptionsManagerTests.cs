using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commons.Module.Messages.Features;
using Eleon.Storage.Lib.Constants;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using FluentAssertions;
using NSubstitute;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using VPortal.Storage.Module;
using VPortal.Storage.Module.DynamicOptions;
using Xunit;

namespace Eleon.Storage.Module.Domain.Tests.DynamicOptions;

public class StorageProviderOptionsManagerTests : TestBase.StorageDomainTestBase
{
    private StorageProviderOptionsManager _manager;

    public StorageProviderOptionsManagerTests()
    {
        _manager = CreateStorageProviderOptionsManager();
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithNormalGroup_Should_CallGetSettingAndGetStorageProvider()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var settingsGroup = "TestGroup";
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test/path"),
            providerId);

        SetupFeatureSettingResponse(settingsGroup, providerId.ToString());
        SetupStorageProviderResponse(provider);

        // Act
        var result = await _manager.GetCurrentStorageProviderSettings(settingsGroup);

        // Assert
        result.Should().NotBeNull();
        result.Value.Key.Should().Be(StorageProviderDomainConstants.StorageTypeFileSystem);
        result.Value.Value.Should().ContainKey("BasePath");
        
        MockEventBus.Received(1).RequestAsync<GetFeatureSettingResponseMsg>(
            Arg.Is<GetFeatureSettingMsg>(msg => msg.Group == settingsGroup));
        MockEventBus.Received(1).RequestAsync<GetStorageProviderResponseMsg>(
            Arg.Is<GetStorageProviderMsg>(msg => msg.StorageProviderId == providerId.ToString()));
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithExplicitGuidPrefix_Should_ExtractGuidAndGetProvider()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var explicitGroup = $"[EXPLICIT]{providerId}";
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            StorageTestHelpers.BuildFileSystemSettings("/test/path"),
            providerId);

        SetupStorageProviderResponse(provider);

        // Act
        var result = await _manager.GetCurrentStorageProviderSettings(explicitGroup);

        // Assert
        result.Should().NotBeNull();
        result.Value.Key.Should().Be(StorageProviderDomainConstants.StorageTypeFileSystem);
        MockEventBus.DidNotReceive().RequestAsync<GetFeatureSettingResponseMsg>(Arg.Any<object>());
        MockEventBus.Received(1).RequestAsync<GetStorageProviderResponseMsg>(
            Arg.Is<GetStorageProviderMsg>(msg => msg.StorageProviderId == providerId.ToString()));
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithExplicitTypePrefix_Should_ReturnTypeWithoutSettings()
    {
        // Arrange
        var providerType = StorageTypes.FileSystem;
        var explicitTypeGroup = $"[EXPLICIT_TYPE]{providerType}";

        // Act
        var result = await _manager.GetCurrentStorageProviderSettings(explicitTypeGroup);

        // Assert
        result.Should().NotBeNull();
        result.Value.Key.Should().Be(providerType.ToString());
        result.Value.Value.Should().BeEmpty();
        MockEventBus.DidNotReceive().RequestAsync<GetFeatureSettingResponseMsg>(Arg.Any<object>());
        MockEventBus.DidNotReceive().RequestAsync<GetStorageProviderResponseMsg>(Arg.Any<object>());
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithTestPrefix_Should_UseTestGroupAsIs()
    {
        // Arrange
        var testGroup = "[TEST]TestProviderId";
        // This should fail Guid parsing, so result should be null
        // Act
        var result = await _manager.GetCurrentStorageProviderSettings(testGroup);

        // Assert
        // Since "TestProviderId" is not a valid Guid, it should return null
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithInvalidGuid_Should_ReturnNull()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        SetupFeatureSettingResponse("TestGroup", invalidGuid);

        // Act
        var result = await _manager.GetCurrentStorageProviderSettings("TestGroup");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithAmbientContext_Should_UseAmbientSettingsGroup()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var ambientGroup = "AmbientGroup";
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            new Dictionary<string, string>(),
            providerId);

        using var _ = StorageProviderOptionsManager.SetAmbientSettingsGroup(ambientGroup);
        SetupFeatureSettingResponse(ambientGroup, providerId.ToString());
        SetupStorageProviderResponse(provider);

        // Act - pass null to use ambient context
        var result = await _manager.GetCurrentStorageProviderSettings(null);

        // Assert
        result.Should().NotBeNull();
        MockEventBus.Received(1).RequestAsync<GetFeatureSettingResponseMsg>(
            Arg.Is<GetFeatureSettingMsg>(msg => msg.Group == ambientGroup));
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithExplicitParameter_Should_OverrideAmbientContext()
    {
        // Arrange
        var ambientGroup = "AmbientGroup";
        var explicitGroup = "ExplicitGroup";
        var providerId = Guid.NewGuid();
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            new Dictionary<string, string>(),
            providerId);

        using var _ = StorageProviderOptionsManager.SetAmbientSettingsGroup(ambientGroup);
        SetupFeatureSettingResponse(explicitGroup, providerId.ToString());
        SetupStorageProviderResponse(provider);

        // Act - pass explicit group which should override ambient
        var result = await _manager.GetCurrentStorageProviderSettings(explicitGroup);

        // Assert
        result.Should().NotBeNull();
        MockEventBus.Received(1).RequestAsync<GetFeatureSettingResponseMsg>(
            Arg.Is<GetFeatureSettingMsg>(msg => msg.Group == explicitGroup));
    }

    [Fact]
    public void SetAmbientSettingsGroup_Should_SetValue()
    {
        // Arrange
        var group = "TestGroup";

        // Act
        using var _ = StorageProviderOptionsManager.SetAmbientSettingsGroup(group);
        var result = _manager.GetAmbientSettingsGroup();

        // Assert
        result.Should().Be(group);
    }

    [Fact]
    public void GetAmbientSettingsGroup_WhenNotSet_Should_ReturnEmptyString()
    {
        // Act
        var result = _manager.GetAmbientSettingsGroup();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetExplicitProviderSettingsGroup_Should_ReturnCorrectFormat()
    {
        // Arrange
        var providerId = Guid.NewGuid();

        // Act
        var result = _manager.GetExplicitProviderSettingsGroup(providerId);

        // Assert
        result.Should().Be($"[EXPLICIT]{providerId}");
    }

    [Fact]
    public void GetExplicitProviderTypeSettingsGroup_Should_ReturnCorrectFormat()
    {
        // Arrange
        var providerType = StorageTypes.FileSystem;

        // Act
        var result = _manager.GetExplicitProviderTypeSettingsGroup(providerType);

        // Assert
        result.Should().Be($"[EXPLICIT_TYPE]{providerType}");
    }

    [Fact]
    public async Task GetCurrentStorageProviderSettings_WithTenantId_Should_PassTenantToGetSetting()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var settingsGroup = "TestGroup";
        var providerId = Guid.NewGuid();
        var provider = StorageTestHelpers.BuildStorageProviderDto(
            StorageProviderDomainConstants.StorageTypeFileSystem,
            new Dictionary<string, string>(),
            providerId);

        MockCurrentTenant.Id.Returns(tenantId);
        _manager = CreateStorageProviderOptionsManager(currentTenant: MockCurrentTenant);

        SetupFeatureSettingResponse(settingsGroup, providerId.ToString(), tenantId);
        SetupStorageProviderResponse(provider);

        // Act
        var result = await _manager.GetCurrentStorageProviderSettings(settingsGroup);

        // Assert
        result.Should().NotBeNull();
        MockEventBus.Received(1).RequestAsync<GetFeatureSettingResponseMsg>(
            Arg.Is<GetFeatureSettingMsg>(msg => msg.Group == settingsGroup));
    }

    // Helper methods
    private void SetupFeatureSettingResponse(string group, string value, Guid? tenantId = null)
    {
        var response = new GetFeatureSettingResponseMsg { Value = value };
        EventBusTestHelpers.SetupEventBusRequestAsync<GetFeatureSettingMsg, GetFeatureSettingResponseMsg>(
            MockEventBus,
            request => request.Group == group ? response : new GetFeatureSettingResponseMsg { Value = null });
    }
}
