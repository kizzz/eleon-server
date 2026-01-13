using System;
using System.Collections.Generic;
using Common.EventBus.Module;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using VPortal.ApplicationConfiguration.Module.DomainServices;

namespace Eleon.ApplicationConfiguration.Module.Test.TestBase;

public abstract class DomainTestBase : MockingTestBase
{
    protected ApplicationConfigurationDomainService CreateApplicationConfigurationDomainService(
        IVportalLogger<ApplicationConfigurationDomainService> logger = null,
        IDistributedEventBus eventBus = null,
        IMemoryCache cache = null,
        IConfiguration configuration = null,
        AbpApplicationConfigurationAppService abpApplicationConfigurationAppService = null,
        ICurrentUser currentUser = null,
        ICurrentTenant currentTenant = null)
    {
        // AbpApplicationConfigurationAppService is a concrete class that can't be mocked
        // Since it's not actually used in the code (commented out), we create a minimal stub
        var abpService = abpApplicationConfigurationAppService ?? CreateStubAbpApplicationConfigurationAppService();
        currentTenant = currentTenant ?? CreateMockCurrentTenant();

        var service = new ApplicationConfigurationDomainService(
            logger ?? CreateMockLogger<ApplicationConfigurationDomainService>(),
            eventBus ?? CreateMockEventBus(),
            cache ?? CreateMemoryCache(),
            configuration ?? CreateMockConfiguration(),
            abpService,
            currentUser ?? CreateMockCurrentUser());

        // Set up LazyServiceProvider to provide ICurrentTenant for DomainService base class
        var lazyServiceProvider = Substitute.For<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>().Returns(currentTenant);
        lazyServiceProvider.LazyGetService<ICurrentTenant>().Returns(currentTenant);

        var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService).GetProperty(
            "LazyServiceProvider", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        lazyServiceProviderProp?.SetValue(service, lazyServiceProvider);

        // Also set _currentTenant field directly as a fallback
        var tenantField = typeof(Volo.Abp.Domain.Services.DomainService).GetField("_currentTenant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tenantField?.SetValue(service, currentTenant);

        return service;
    }

    private AbpApplicationConfigurationAppService CreateStubAbpApplicationConfigurationAppService()
    {
        // AbpApplicationConfigurationAppService is a concrete class from ABP Framework that can't be easily mocked.
        // Since it's not actually used in ApplicationConfigurationDomainService (the call is commented out at line 60-63),
        // and the field is only stored but never accessed, we can use a workaround.
        // We'll create a minimal test wrapper class that extends AbpApplicationConfigurationAppService.
        // However, since we can't easily extend it, we'll use a different approach:
        // Create a test-specific implementation that satisfies the type requirement.
        
        // Since the service is never called, we can create a minimal instance using FormatterServices
        // which creates an uninitialized object without calling constructors.
        try
        {
            var type = typeof(AbpApplicationConfigurationAppService);
            var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
            return (AbpApplicationConfigurationAppService)instance;
        }
        catch
        {
            // If FormatterServices fails, we need to provide a real instance.
            // Since this is complex, we'll throw a clear error message.
            throw new InvalidOperationException(
                "Cannot create AbpApplicationConfigurationAppService instance for testing. " +
                "This service is not actually used in ApplicationConfigurationDomainService " +
                "(the call is commented out). Consider refactoring to use an interface or " +
                "provide a test-specific implementation.");
        }
    }
}

