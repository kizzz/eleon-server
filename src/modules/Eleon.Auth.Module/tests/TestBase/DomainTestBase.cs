using System.Collections.Generic;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using Common.EventBus.Module;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using VPortal.Identity.Module.DomainServices;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

public abstract class DomainTestBase : MockingTestBase
{
    protected IMemoryCache CreateMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    protected ApiKeyValidator BuildApiKeyValidator(
        IResponseCapableEventBus eventBus = null,
        IMemoryCache memoryCache = null,
        IConfiguration configuration = null)
    {
        eventBus ??= CreateMockResponseCapableEventBus();
        memoryCache ??= CreateMemoryCache();
        configuration ??= new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ApiKey:NotValidatedSecretKeys", "[]" }
            })
            .Build();
        
        var logger = CreateMockLogger<ApiKeyValidator>();
        return new ApiKeyValidator(logger, (IDistributedEventBus)eventBus, memoryCache, configuration);
    }

    protected SignInOtpManager BuildSignInOtpManager(
        IDistributedEventBus eventBus = null,
        IStringLocalizer<Volo.Abp.Identity.Localization.IdentityResource> localizer = null,
        IConfiguration configuration = null,
        ISessionAccessor sessionAccessor = null)
    {
        eventBus ??= CreateMockEventBus();
        localizer ??= Substitute.For<IStringLocalizer<Volo.Abp.Identity.Localization.IdentityResource>>();
        localizer["OtpMessage"].Returns(new LocalizedString("OtpMessage", "Otp message"));
        configuration ??= new ConfigurationBuilder().Build();
        sessionAccessor ??= new StubSessionAccessor(new FullSessionInformation { SessionId = "session" });
        
        return new SignInOtpManager(eventBus, localizer, configuration, sessionAccessor);
    }

    private sealed class StubSessionAccessor(FullSessionInformation session) : ISessionAccessor
    {
        public FullSessionInformation Session { get; } = session;
    }
}
