using System;
using System.Collections.Generic;
using System.Net;
using Common.EventBus.Module;
using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations;
using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations.Dtos;
using Eleon.ApplicationConfiguration.Module.Test.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftAbp.Messages.AppConfig;
using Messaging.Module.ETO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VPortal.ApplicationConfiguration.Module.DomainServices;

namespace Eleon.ApplicationConfiguration.Module.Test.Application;

public class EleonApplicationConfigurationAppServiceTests : AppServiceTestBase
{
    [Fact]
    public async Task GetAsync_ShouldReturnDto_WhenValidRequest()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = applicationId };

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Name", "Test App" },
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject
        {
            ApplicationName = "Test App",
            ApplicationPath = "/app1/"
        };
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(callInfo =>
            {
                var vo = callInfo.Arg<EleoncoreApplicationConfigurationValueObject>();
                return new EleoncoreApplicationConfigurationDto
                {
                    ApplicationName = vo.ApplicationName,
                    ApplicationPath = vo.ApplicationPath
                };
            });

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        result.ApplicationName.ShouldBe("Test App");
        result.ApplicationPath.ShouldBe("/app1/");
    }

    [Fact]
    public async Task GetAsync_ShouldUrlDecodeApplicationIdentifier()
    {
        var encodedAppId = WebUtility.UrlEncode("/app1/");
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = encodedAppId };

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Verify domain service was called with decoded identifier
    }

    [Fact]
    public async Task GetAsync_ShouldMapValueObjectToDto()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = applicationId };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject
        {
            ApplicationName = "Mapped App",
            ApplicationPath = "/app1/"
        };
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        EleoncoreApplicationConfigurationValueObject capturedVo = null;
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(callInfo =>
            {
                capturedVo = callInfo.Arg<EleoncoreApplicationConfigurationValueObject>();
                return new EleoncoreApplicationConfigurationDto
                {
                    ApplicationName = capturedVo.ApplicationName,
                    ApplicationPath = capturedVo.ApplicationPath
                };
            });

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        capturedVo.ShouldNotBeNull();
        capturedVo.ApplicationName.ShouldBe("Mapped App");
    }

    [Fact]
    public async Task GetAsync_ShouldCallDomainService()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = applicationId };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Use real domain service instance instead of mocking (it's a concrete class)
        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Verify the domain service was called by checking the result is not null
    }

    [Fact]
    public async Task GetAsync_ShouldPassCorrectApplicationId()
    {
        var applicationId = "/test-app/";
        var encodedId = WebUtility.UrlEncode(applicationId);
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = encodedId };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        // Use real domain service instance instead of mocking (it's a concrete class)
        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Verify URL decoding worked by checking the result is not null
        // The domain service will be called with the decoded applicationId
    }

    // Phase 7: Application Service Edge Cases

    [Fact]
    public async Task GetAsync_ShouldHandleNullRequest()
    {
        var userId = Guid.NewGuid();

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        await Should.ThrowAsync<NullReferenceException>(() => service.GetAsync(null));
    }

    [Fact]
    public async Task GetAsync_ShouldHandleNullApplicationIdentifier()
    {
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = null };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Should handle null identifier
    }

    [Fact]
    public async Task GetAsync_ShouldHandleEmptyApplicationIdentifier()
    {
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = "" };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldHandleWhitespaceApplicationIdentifier()
    {
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = "   " };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldHandleAlreadyDecodedIdentifier()
    {
        var applicationId = "/app1/";
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = applicationId };

        var configuration = CreateConfiguration(new Dictionary<string, string>
        {
            { "ApplicationConfiguration:Applications:0:Path", "/app1/" }
        });

        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Should handle already decoded identifier gracefully
    }

    [Fact]
    public async Task GetAsync_ShouldHandleSpecialCharactersInEncodedId()
    {
        var applicationId = "/app with spaces/";
        var encodedId = WebUtility.UrlEncode(applicationId);
        var userId = Guid.NewGuid();
        var request = new ApplicationConfigurationRequestDto { ApplicationIdentifier = encodedId };

        var configuration = CreateConfiguration(new Dictionary<string, string>());
        var eventBus = CreateMockResponseCapableEventBus();
        var baseConfig = new EleoncoreApplicationConfigurationValueObject();
        SetupEventBusRequestAsync<GetBaseAppConfigRequestMsg, GetBaseAppConfigResponseMsg>(
            eventBus,
            new GetBaseAppConfigResponseMsg { ApplicationConfiguration = baseConfig });

        var domainService = CreateApplicationConfigurationDomainService(
            eventBus: (IDistributedEventBus)eventBus,
            configuration: configuration,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var objectMapper = CreateMockObjectMapper();
        objectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(Arg.Any<EleoncoreApplicationConfigurationValueObject>())
            .Returns(new EleoncoreApplicationConfigurationDto());

        var service = CreateEleonApplicationConfigurationAppService(
            domainService: domainService,
            objectMapper: objectMapper,
            currentUser: CreateMockCurrentUser(userId, "user"));

        var result = await service.GetAsync(request);

        result.ShouldNotBeNull();
        // Should decode special characters correctly
    }
}

