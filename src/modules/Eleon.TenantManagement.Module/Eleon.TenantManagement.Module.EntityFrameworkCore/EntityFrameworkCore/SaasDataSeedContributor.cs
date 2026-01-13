using Microsoft.Extensions.DependencyInjection;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.Saas;

public class SaasDataSeedContributor : IDataSeedContributor, ITransientDependency
{
  private readonly TenantHostnameDomainService tenantHostnameDomainService;
  private readonly IServiceProvider serviceProvider;
  private readonly ICurrentTenant currentTenant;

  public SaasDataSeedContributor(
      TenantHostnameDomainService tenantHostnameDomainService,
      IServiceProvider serviceProvider,
      ICurrentTenant currentTenant
      )
  {
    this.tenantHostnameDomainService = tenantHostnameDomainService;
    this.serviceProvider = serviceProvider;
    this.currentTenant = currentTenant;
  }


  public virtual async Task SeedAsync(DataSeedContext context)
  {
    if (context == null)
    {
      return;
    }

    using (currentTenant.Change(null))
    {
      await tenantHostnameDomainService.UpdateInternalHostAndTenantsHostnamesAsync();
    }
    //using (currentTenant.ChangeDefault())
    //{
    //    await tenantHostnameDomainService.UpdateInternalHostAndTenantsHostnamesAsync();
    //}
  }
  private Tenant GetTenantDetails(Guid? id)
  {
    using (currentTenant.Change(null))
    {
      var tenantManagementDbContext = serviceProvider.GetService<Volo.Abp.TenantManagement.EntityFrameworkCore.ITenantManagementDbContext>();
      return tenantManagementDbContext
          .Tenants
          .IncludeDetails(false)
          .OrderBy(t => t.Id)
          .FirstOrDefault(t => t.Id == id);
    }
  }
}
