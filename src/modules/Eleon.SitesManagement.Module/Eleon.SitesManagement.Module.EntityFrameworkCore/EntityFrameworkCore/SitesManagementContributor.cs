using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.SitesManagement.Module.Repositories;


namespace VPortal.SitesManagement.Module.EntityFrameworkCore;
public class SitesManagementContributor : IDataSeedContributor, ITransientDependency
{
  private readonly ClientApplicationDomainService clientApplicationDomainService;
  private readonly MicroserviceManager microserviceDomainService;
  private readonly UiModuleManager uiModuleManager;
  private readonly ResourceDomainService resourceDomainService;
  private readonly IGuidGenerator guidGenerator;

  public SitesManagementContributor(
      ClientApplicationDomainService clientApplicationDomainService,
      MicroserviceManager microserviceDomainService,
      UiModuleManager uiModuleManager,
      ResourceDomainService resourceDomainService,
      IGuidGenerator guidGenerator
      )
  {
    this.clientApplicationDomainService = clientApplicationDomainService;
    this.microserviceDomainService = microserviceDomainService;
    this.uiModuleManager = uiModuleManager;
    this.resourceDomainService = resourceDomainService;
    this.guidGenerator = guidGenerator;
  }


  public virtual async Task SeedAsync(DataSeedContext context)
  {

    //var systemResources = new ModuleEntity(guidGenerator.Create())
    //{
    //    Path = "/resources",
    //    Type = Common.Module.Constants.ModuleType.Resource,
    //    DisplayName = "System Resources",
    //    IsEnabled = true,
    //};

    //await resourceDomainService.CreateResource(systemResources);

  }
}


