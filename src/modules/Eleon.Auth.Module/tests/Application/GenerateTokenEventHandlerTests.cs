using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.Identity;
using EleonsoftAbp.Messages.ApiKey;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Messaging.Module.ETO;
using ModuleCollector.Identity.Module.Identity.Module.Application.EventServices;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class GenerateTokenEventHandlerTests : DomainTestBase
{
    private static string ComputeSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
    private GenerateTokenEventHandler BuildService(
        IClientStore clientStore,
        IResourceStore resourceStore,
        ITokenService tokenService,
        ApiKeyValidator apiKeyValidator = null)
    {
        var logger = CreateMockLogger<GenerateTokenEventHandler>();
        var responseContext = Substitute.For<IResponseContext>();
        var identitySecurityLogManager = BuildIdentitySecurityLogManager(new TestIdentityUserManager());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new[] { new KeyValuePair<string, string>("App:Authority", "https://example.com") })
            .Build();

        return new GenerateTokenEventHandler(
            logger,
            responseContext,
            clientStore,
            resourceStore,
            tokenService,
            apiKeyValidator ?? BuildApiKeyValidator(),
            identitySecurityLogManager,
            config);
    }

    [Fact]
    public async Task HandleEventAsync_ValidRequest_GeneratesToken()
    {
        var client = new Client
        {
            ClientId = "test-client",
            ClientSecrets = { new Secret(ComputeSha256("secret")) },
            RequireClientSecret = true,
            AllowedScopes = { "api" },
            AccessTokenLifetime = 3600
        };

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync("test-client").Returns(Task.FromResult(client));

        var resourceStore = TestResourceStore.CreateDefault();
        var tokenService = Substitute.For<ITokenService>();
        tokenService.CreateSecurityTokenAsync(Arg.Any<Token>()).Returns("generated-token");

        var eventBus = CreateMockResponseCapableEventBus();
        var apiKey = new IdentityApiKeyEto { Id = Guid.NewGuid(), Name = "test-key", RefId = Guid.NewGuid().ToString() };
        SetupEventBusRequestAsync<object, ValidApiKeyReponseMsg>(
            eventBus,
            new ValidApiKeyReponseMsg { ApiKey = apiKey });
        
        var apiKeyValidator = BuildApiKeyValidator(eventBus);

        var service = BuildService(clientStore, resourceStore, tokenService, apiKeyValidator);
        var eventData = new GenerateTokenRequestMsg
        {
            ClientId = "test-client",
            ClientSecret = "secret",
            ApiKey = "api-key",
            Nonce = "nonce",
            Timestamp = DateTime.UtcNow,
            Signature = "signature"
        };

        await service.HandleEventAsync(eventData);
    }

    [Fact]
    public async Task HandleEventAsync_InvalidUser_ThrowsException()
    {
        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<Client>(null));

        var resourceStore = TestResourceStore.CreateDefault();
        var tokenService = Substitute.For<ITokenService>();
        var eventBus = CreateMockResponseCapableEventBus();
        SetupEventBusRequestAsync<object, ValidApiKeyReponseMsg>(
            eventBus,
            new ValidApiKeyReponseMsg { ApiKey = null });
        var apiKeyValidator = BuildApiKeyValidator(eventBus);

        var service = BuildService(clientStore, resourceStore, tokenService, apiKeyValidator);
        var eventData = new GenerateTokenRequestMsg
        {
            ClientId = "invalid-client",
            ClientSecret = "secret"
        };

        await service.HandleEventAsync(eventData);
    }

    [Fact]
    public async Task HandleEventAsync_InvalidClient_ThrowsException()
    {
        var client = new Client
        {
            ClientId = "test-client",
            RequireClientSecret = true,
            ClientSecrets = { new Secret(ComputeSha256("wrong-secret")) }
        };

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync("test-client").Returns(Task.FromResult(client));

        var resourceStore = TestResourceStore.CreateDefault();
        var tokenService = Substitute.For<ITokenService>();
        var eventBus = CreateMockResponseCapableEventBus();
        SetupEventBusRequestAsync<object, ValidApiKeyReponseMsg>(
            eventBus,
            new ValidApiKeyReponseMsg { ApiKey = null });
        var apiKeyValidator = BuildApiKeyValidator(eventBus);

        var service = BuildService(clientStore, resourceStore, tokenService, apiKeyValidator);
        var eventData = new GenerateTokenRequestMsg
        {
            ClientId = "test-client",
            ClientSecret = "wrong-secret"
        };

        await service.HandleEventAsync(eventData);
    }
}

