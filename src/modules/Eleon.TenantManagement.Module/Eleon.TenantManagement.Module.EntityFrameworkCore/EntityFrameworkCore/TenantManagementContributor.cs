using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;


namespace VPortal.TenantManagement.Module.EntityFrameworkCore;
public class TenantManagementContributor : IDataSeedContributor, ITransientDependency
{
  private readonly IGuidGenerator guidGenerator;

  public TenantManagementContributor(
      IGuidGenerator guidGenerator
      )
  {
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
