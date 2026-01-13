using System;
using System.Collections.Generic;
using System.Linq;
using Common.EventBus.Module;
using Eleon.ApplicationConfiguration.Module.Test.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftAbp.Messages.AppConfig;
using Messaging.Module.ETO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ModuleCollector.Commons.Module.Proxy.Constants;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using NSubstitute;
using Shouldly;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;
using VPortal.ApplicationConfiguration.Module.DomainServices;
// Enum types are accessed via ClientApplicationEto and ApplicationModuleEto properties

namespace Eleon.ApplicationConfiguration.Module.Test.Domain.DomainServices;

public class ApplicationConfigurationDomainServiceTests : DomainTestBase
{
    [Fact]
    public async Task GetAsync_ShouldReturnConfiguration_WhenAppFoundInAppsettings()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:FrameworkType", "Angular" },
            { "ApplicationConfiguration:Applications:0:StyleType", "PrimeNg" },
            { "ApplicationConfiguration:Applications:0:ClientApplicationType", "Portal" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        // Set CurrentTenant using reflection
        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBe("Test App");
        result.ApplicationPath.ShouldBe("/app1/");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnConfiguration_WhenAppNotFoundInAppsettings()
    {
        var applicationId = "/unknown/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEnrichedConfiguration_WhenEnrichmentEnabled()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var enrichedApp = new ClientApplicationEto
        {
            Name = "Enriched App",
            Path = "/app1/",
            Modules = new List<ApplicationModuleEto>
            {
                new ApplicationModuleEto { Name = "Module1", Url = "/module1" }
            }
        };

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto> { enrichedApp }
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ClientApplication.ShouldNotBeNull();
        result.ClientApplication.Name.ShouldBe("Enriched App");
        result.Modules.ShouldNotBeNull();
        result.Modules.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnBaseConfiguration_WhenEnrichmentDisabled()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "false" },
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should not call enrichment
        await ((IResponseCapableEventBus)eventBus).DidNotReceive().RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Any<int>());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnBaseConfiguration_WhenEnrichmentFails()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Enrichment returns failure
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = false,
                Applications = null
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should still return base config even if enrichment fails
    }

    [Fact]
    public async Task GetAsync_ShouldCacheConfiguration_WhenFirstCall()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        var cacheKey = $"AppConfig:{tenantId}:{applicationId}";
        cache.TryGetValue(cacheKey, out var cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldUseCorrectCacheKey_WithTenantAndAppId()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Cache key format: "AppConfig:{CurrentTenant.Id ?? Guid.Empty}:{applicationId}"
        var expectedCacheKey = $"AppConfig:{tenantId}:{applicationId}";
        cache.TryGetValue(expectedCacheKey, out var cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldRequestEnrichment_WhenEnabled()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>()
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        await service.GetAsync(applicationId);

        await ((IResponseCapableEventBus)eventBus).Received(1).RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Any<int>());
    }

    [Fact]
    public async Task GetAsync_ShouldReadFromAppsettings_WhenConfigured()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "My App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:FrameworkType", "Angular" },
            { "ApplicationConfiguration:Applications:0:StyleType", "PrimeNg" },
            { "ApplicationConfiguration:Applications:0:ClientApplicationType", "Portal" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBe("My App");
        result.ApplicationPath.ShouldBe("/app1/");
    }

    [Fact]
    public async Task GetAsync_ShouldMapModules_WhenPresent()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Url", "/module1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:PluginName", "Plugin1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Expose", "./Module1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:OrderIndex", "1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:LoadLevel", "Auto" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.Modules.ShouldNotBeNull();
        result.Modules.Count.ShouldBe(1);
        result.Modules[0].Url.ShouldBe("/module1");
        result.Modules[0].PluginName.ShouldBe("Plugin1");
    }

    [Fact]
    public async Task GetAsync_ShouldSetCurrentUser_WhenAuthenticated()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var userName = "testuser";

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, userName);
        currentUser.IsAuthenticated.Returns(true);
        currentUser.Email.Returns("test@example.com");
        currentUser.Name.Returns("Test");
        currentUser.SurName.Returns("User");

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.Id.ShouldBe(userId);
        result.CurrentUser.UserName.ShouldBe(userName);
        result.CurrentUser.IsAuthenticated.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAsync_ShouldSetCurrentTenant_WhenAvailable()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var tenantName = "TestTenant";

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, tenantName);
        currentTenant.IsAvailable.Returns(true);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentTenant.ShouldNotBeNull();
        result.CurrentTenant.Id.ShouldBe(tenantId);
        result.CurrentTenant.Name.ShouldBe(tenantName);
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullApplicationId()
    {
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(null);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldNormalizeAppIdPaths()
    {
        var applicationId = "app1"; // Without slashes
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should match because paths are normalized
    }

    [Fact]
    public async Task GetAsync_ShouldHandleCaseInsensitiveAppId()
    {
        var applicationId = "/APP1/"; // Different case
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should match because comparison is case-insensitive
    }

    [Fact]
    public async Task GetAsync_ShouldCallGetBaseAppConfig_FromEventBus()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        await service.GetAsync(applicationId);

        await ((IResponseCapableEventBus)eventBus).Received(1).RequestAsync<GetBaseAppConfigResponseMsg>(
            Arg.Is<GetBaseAppConfigRequestMsg>(msg => msg.UserId == userId), Arg.Any<int>());
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEnrichmentWithEmptyApplications()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>() // Empty list
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should still return base config
    }

    // Phase 1: Error Handling and Exception Scenarios

    [Fact]
    public async Task GetAsync_ShouldHandleEventBusException_WhenGetBaseAppConfigFails()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        
        // Event bus throws exception
        ((IResponseCapableEventBus)eventBus).RequestAsync<GetBaseAppConfigResponseMsg>(
            Arg.Any<GetBaseAppConfigRequestMsg>(), Arg.Any<int>())
            .Returns<Task<GetBaseAppConfigResponseMsg>>(callInfo => throw new Exception("Event bus error"));

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        await Should.ThrowAsync<Exception>(() => service.GetAsync(applicationId));
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEventBusException_WhenEnrichmentFails()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Enrichment throws exception
        ((IResponseCapableEventBus)eventBus).RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Any<int>())
            .Returns<Task<EnrichedApplicationConfigurationResponseMsg>>(callInfo => throw new Exception("Enrichment error"));

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should fallback to base config when enrichment throws exception
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullGetBaseAppConfigResponse()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        
        // Return null response - use explicit cast to avoid ambiguity
        GetBaseAppConfigResponseMsg nullResponse = null;
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            nullResponse);

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        await Should.ThrowAsync<NullReferenceException>(() => service.GetAsync(applicationId));
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullApplicationConfigurationInResponse()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        
        // Response with null ApplicationConfiguration
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = null });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        await Should.ThrowAsync<NullReferenceException>(() => service.GetAsync(applicationId));
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullEnrichmentResponse()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Null enrichment response - use explicit cast to avoid ambiguity
        EnrichedApplicationConfigurationResponseMsg nullEnrichmentResponse = null;
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            nullEnrichmentResponse);

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should handle null response gracefully
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullApplicationsList()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Response with null Applications list
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = null
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should handle null Applications list
    }

    [Fact]
    public async Task GetAsync_ShouldUseCustomTimeout_WhenConfigured()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:EnrichmentTimeoutSeconds", "120" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>()
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Verify custom timeout (120 seconds) was used
        await ((IResponseCapableEventBus)eventBus).Received(1).RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Is<int>(timeout => timeout == 120));
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultEnrichmentTimeout_WhenNotConfigured()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>()
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Verify default timeout (60 seconds) was used
        await ((IResponseCapableEventBus)eventBus).Received(1).RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Is<int>(timeout => timeout == 60));
    }

    // Phase 2: Configuration Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleMissingAppsettingsSection()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        // Configuration without ApplicationConfiguration:Applications section
        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEmptyAppsettingsSection()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        // Configuration with empty applications list
        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications", "[]" }
        });
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultFrameworkType_WhenInvalidEnum()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:FrameworkType", "InvalidFramework" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ClientApplication.ShouldNotBeNull();
        result.ClientApplication.FrameworkType.ToString().ShouldBe("Angular");
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultStyleType_WhenInvalidEnum()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:StyleType", "InvalidStyle" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ClientApplication.ShouldNotBeNull();
        result.ClientApplication.StyleType.ToString().ShouldBe("PrimeNg");
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultClientApplicationType_WhenInvalidEnum()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:ClientApplicationType", "InvalidType" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ClientApplication.ShouldNotBeNull();
        result.ClientApplication.ClientApplicationType.ToString().ShouldBe("Portal");
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultLoadLevel_WhenInvalidEnum()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:Modules:0:LoadLevel", "InvalidLoadLevel" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Expose", "./Module1" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.Modules.ShouldNotBeNull();
        if (result.Modules.Count > 0)
        {
            result.Modules[0].LoadLevel.ToString().ShouldBe("Auto");
        }
    }

    [Fact]
    public async Task GetAsync_ShouldHandleModuleWithShortExpose()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Expose", "X" }, // Only 1 character
            { "ApplicationConfiguration:Applications:0:Modules:0:PluginName", "Plugin1" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        // Should throw ArgumentOutOfRangeException when Substring(2) is called on string shorter than 2
        await Should.ThrowAsync<System.ArgumentOutOfRangeException>(() => service.GetAsync(applicationId));
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullModulesList()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:Name", "Test App" }
            // No Modules property - will be null
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.Modules.ShouldNotBeNull();
        result.Modules.Count.ShouldBe(0);
    }

    [Fact]
    public async Task GetAsync_ShouldMapMultipleModules()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Url", "/module1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:PluginName", "Plugin1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:Expose", "./Module1" },
            { "ApplicationConfiguration:Applications:0:Modules:0:OrderIndex", "1" },
            { "ApplicationConfiguration:Applications:0:Modules:1:Url", "/module2" },
            { "ApplicationConfiguration:Applications:0:Modules:1:PluginName", "Plugin2" },
            { "ApplicationConfiguration:Applications:0:Modules:1:Expose", "./Module2" },
            { "ApplicationConfiguration:Applications:0:Modules:1:OrderIndex", "2" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.Modules.ShouldNotBeNull();
        result.Modules.Count.ShouldBe(2);
        result.Modules[0].Url.ShouldBe("/module1");
        result.Modules[1].Url.ShouldBe("/module2");
    }

    [Fact]
    public async Task GetAsync_ShouldUseDefaultCacheTtl_WhenNotConfigured()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Verify cache was set with default TTL (10 minutes)
        var cacheKey = $"AppConfig:{tenantId}:{applicationId}";
        cache.TryGetValue(cacheKey, out var cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldUseCustomCacheTtl_WhenConfigured()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:CacheTtlMinutes", "30" }
        });
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Verify cache was set
        var cacheKey = $"AppConfig:{tenantId}:{applicationId}";
        cache.TryGetValue(cacheKey, out var cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();
    }

    // Phase 3: CurrentUser Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleUnauthenticatedUser()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        currentUser.IsAuthenticated.Returns(false);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAsync_ShouldSetAllUserProperties_WhenAuthenticated()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "testuser", tenantId);
        currentUser.IsAuthenticated.Returns(true);
        currentUser.Name.Returns("Test");
        currentUser.SurName.Returns("User");
        currentUser.Email.Returns("test@example.com");
        currentUser.EmailVerified.Returns(true);
        currentUser.PhoneNumber.Returns("+1234567890");
        currentUser.PhoneNumberVerified.Returns(true);
        currentUser.Roles.Returns(new[] { "Admin", "User" });
        
        // FindSessionId is an extension method that calls FindClaim
        // Mock FindClaim to return the session claim
        var sessionClaim = new System.Security.Claims.Claim(Volo.Abp.Security.Claims.AbpClaimTypes.SessionId, "session123");
        currentUser.FindClaim(Volo.Abp.Security.Claims.AbpClaimTypes.SessionId).Returns(sessionClaim);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.Id.ShouldBe(userId);
        result.CurrentUser.UserName.ShouldBe("testuser");
        result.CurrentUser.Name.ShouldBe("Test");
        result.CurrentUser.SurName.ShouldBe("User");
        result.CurrentUser.Email.ShouldBe("test@example.com");
        result.CurrentUser.EmailVerified.ShouldBeTrue();
        result.CurrentUser.PhoneNumber.ShouldBe("+1234567890");
        result.CurrentUser.PhoneNumberVerified.ShouldBeTrue();
        result.CurrentUser.Roles.ShouldContain("Admin");
        result.CurrentUser.Roles.ShouldContain("User");
        result.CurrentUser.SessionId.ShouldBe("session123");
    }

    [Fact]
    public async Task GetAsync_ShouldHandleUserWithNullOptionalFields()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        currentUser.IsAuthenticated.Returns(true);
        currentUser.Name.Returns((string)null);
        currentUser.SurName.Returns((string)null);
        currentUser.Email.Returns((string)null);
        currentUser.PhoneNumber.Returns((string)null);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.Name.ShouldBeNull();
        result.CurrentUser.SurName.ShouldBeNull();
        result.CurrentUser.Email.ShouldBeNull();
        result.CurrentUser.PhoneNumber.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldHandleImpersonatedUser()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var impersonatorUserId = Guid.NewGuid();
        var impersonatorTenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        currentUser.IsAuthenticated.Returns(true);
        
        // FindImpersonator* are extension methods that call FindClaim
        // Mock FindClaim to return the impersonator claims
        var impersonatorUserIdClaim = new System.Security.Claims.Claim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorUserId, impersonatorUserId.ToString());
        var impersonatorTenantIdClaim = new System.Security.Claims.Claim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorTenantId, impersonatorTenantId.ToString());
        var impersonatorUserNameClaim = new System.Security.Claims.Claim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorUserName, "impersonator");
        var impersonatorTenantNameClaim = new System.Security.Claims.Claim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorTenantName, "ImpersonatorTenant");
        
        currentUser.FindClaim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorUserId).Returns(impersonatorUserIdClaim);
        currentUser.FindClaim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorTenantId).Returns(impersonatorTenantIdClaim);
        currentUser.FindClaim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorUserName).Returns(impersonatorUserNameClaim);
        currentUser.FindClaim(Volo.Abp.Security.Claims.AbpClaimTypes.ImpersonatorTenantName).Returns(impersonatorTenantNameClaim);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.ImpersonatorUserId.ShouldBe(impersonatorUserId);
        result.CurrentUser.ImpersonatorTenantId.ShouldBe(impersonatorTenantId);
        result.CurrentUser.ImpersonatorUserName.ShouldBe("impersonator");
        result.CurrentUser.ImpersonatorTenantName.ShouldBe("ImpersonatorTenant");
    }

    [Fact]
    public async Task GetAsync_ShouldHandleUserWithoutImpersonation()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        currentUser.IsAuthenticated.Returns(true);
        
        // No impersonation claims - FindClaim returns null
        currentUser.FindClaim(Arg.Any<string>()).Returns((System.Security.Claims.Claim)null);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentUser.ShouldNotBeNull();
        result.CurrentUser.ImpersonatorUserId.ShouldBeNull();
        result.CurrentUser.ImpersonatorTenantId.ShouldBeNull();
        result.CurrentUser.ImpersonatorUserName.ShouldBeNull();
        result.CurrentUser.ImpersonatorTenantName.ShouldBeNull();
    }

    // Phase 4: CurrentTenant Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleNullTenant()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var currentTenant = CreateMockCurrentTenant(null, null); // Host tenant
        currentTenant.IsAvailable.Returns(false);

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.CurrentTenant.ShouldNotBeNull();
        result.CurrentTenant.Id.ShouldBeNull();
        result.CurrentTenant.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAsync_ShouldUseGuidEmpty_WhenTenantIsNull()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var currentTenant = CreateMockCurrentTenant(null, null); // Host tenant

        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Cache key should use Guid.Empty for null tenant
        var expectedCacheKey = $"AppConfig:{Guid.Empty}:{applicationId}";
        cache.TryGetValue(expectedCacheKey, out var cachedValue).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAsync_ShouldIsolateCacheByTenant()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // First tenant
        var currentUser1 = CreateMockCurrentUser(userId, "user", tenantId1);
        var currentTenant1 = CreateMockCurrentTenant(tenantId1, "Tenant1");
        var service1 = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser1,
            currentTenant: currentTenant1);

        await service1.GetAsync(applicationId);

        // Second tenant
        var currentUser2 = CreateMockCurrentUser(userId, "user", tenantId2);
        var currentTenant2 = CreateMockCurrentTenant(tenantId2, "Tenant2");
        var service2 = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser2,
            currentTenant: currentTenant2);

        await service2.GetAsync(applicationId);

        // Both cache entries should exist with different keys
        var cacheKey1 = $"AppConfig:{tenantId1}:{applicationId}";
        var cacheKey2 = $"AppConfig:{tenantId2}:{applicationId}";
        cache.TryGetValue(cacheKey1, out var cachedValue1).ShouldBeTrue();
        cache.TryGetValue(cacheKey2, out var cachedValue2).ShouldBeTrue();
        cachedValue1.ShouldNotBeNull();
        cachedValue2.ShouldNotBeNull();
    }

    // Phase 5: Application ID Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleWhitespaceOnlyApplicationId()
    {
        var applicationId = "   ";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should normalize to "//" and not match any app
    }

    [Fact]
    public async Task GetAsync_ShouldHandleApplicationIdWithMultipleSlashes()
    {
        var applicationId = "///app///";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should normalize and match
    }

    [Fact]
    public async Task GetAsync_ShouldHandleApplicationIdWithTrailingWhitespace()
    {
        var applicationId = " /app1/ ";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should trim and match
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEmptyStringApplicationId()
    {
        var applicationId = "";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should normalize to "//"
    }

    [Fact]
    public async Task GetAsync_ShouldHandleMultipleApplicationsInAppsettings()
    {
        var applicationId = "/app2/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "App 1" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" },
            { "ApplicationConfiguration:Applications:1:Name", "App 2" },
            { "ApplicationConfiguration:Applications:1:Path", "/app2/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBe("App 2");
        result.ApplicationPath.ShouldBe("/app2/");
    }

    // Phase 6: Enrichment Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleEnrichmentAppNotFound()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Enrichment returns apps but none match
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>
                {
                    new ClientApplicationEto { Path = "/other-app/" }
                }
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should not have enriched app since none matched
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEnrichmentWithMultipleApps()
    {
        var applicationId = "/app2/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app2/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Enrichment returns multiple apps, match correct one
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>
                {
                    new ClientApplicationEto { Path = "/app1/", Name = "App 1" },
                    new ClientApplicationEto { Path = "/app2/", Name = "App 2" }
                }
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        result.ClientApplication.ShouldNotBeNull();
        result.ClientApplication.Path.ShouldBe("/app2/");
        result.ClientApplication.Name.ShouldBe("App 2");
    }

    [Fact]
    public async Task GetAsync_ShouldUseEnrichmentCache_WhenAvailable()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var cache = CreateMemoryCache();
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var enrichedApps = new List<ClientApplicationEto>
        {
            new ClientApplicationEto { Path = "/app1/", Name = "Enriched App" }
        };

        // First call - should cache
        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = enrichedApps
            });

        var currentUser = CreateMockCurrentUser(userId, "user", tenantId);
        var currentTenant = CreateMockCurrentTenant(tenantId, "TestTenant");
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            cache: cache,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        await service.GetAsync(applicationId);

        // Second call - should use cache
        await service.GetAsync(applicationId);

        // Should only call event bus once (first call)
        await ((IResponseCapableEventBus)eventBus).Received(1).RequestAsync<EnrichedApplicationConfigurationResponseMsg>(
            Arg.Any<EnrichApplicationsConfigurationRequestMsg>(), Arg.Any<int>());
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEnrichmentWithNullTenantId()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:EnableEnrichment", "true" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        SetupEventBusRequestAsync<EnrichApplicationsConfigurationRequestMsg, EnrichedApplicationConfigurationResponseMsg>(
            eventBus,
            new EnrichedApplicationConfigurationResponseMsg
            {
                IsSuccess = true,
                Applications = new List<ClientApplicationEto>()
            });

        var currentUser = CreateMockCurrentUser(userId, "user");
        var currentTenant = CreateMockCurrentTenant(null, null); // Host tenant
        var service = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: currentUser,
            currentTenant: currentTenant);

        var result = await service.GetAsync(applicationId);

        result.ShouldNotBeNull();
        // Should handle null tenant ID in enrichment request
    }
}

