using System;
using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations;
using Eleon.ApplicationConfiguration.Module.Test.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using NSubstitute;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VPortal.ApplicationConfiguration.Module.DomainServices;

namespace Eleon.ApplicationConfiguration.Module.Test.TestBase;

public abstract class AppServiceTestBase : DomainTestBase
{
    protected EleonApplicationConfigurationAppService CreateEleonApplicationConfigurationAppService(
        IVportalLogger<EleonApplicationConfigurationAppService> logger = null,
        ApplicationConfigurationDomainService domainService = null,
        IObjectMapper objectMapper = null,
        ICurrentUser currentUser = null)
    {
        var service = new EleonApplicationConfigurationAppService(
            logger ?? CreateMockLogger<EleonApplicationConfigurationAppService>(),
            domainService ?? CreateApplicationConfigurationDomainService());

        SetAppServiceDependencies(service, objectMapper ?? CreateMockObjectMapper(), currentUser ?? CreateMockCurrentUser());
        return service;
    }
}


