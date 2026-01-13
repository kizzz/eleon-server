using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.EleoncoreMultiTenancy.Module.Repositories
{
  public class CustomEfCoreTenantRepository : EfCoreTenantRepository, ITenantRepository
  {
    public CustomEfCoreTenantRepository(IDbContextProvider<ITenantManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
      DbContextProvider = dbContextProvider;
    }

    public IDbContextProvider<ITenantManagementDbContext> DbContextProvider { get; }
    protected override async Task<ITenantManagementDbContext> GetDbContextAsync()
    {
      return await DbContextProvider.GetDbContextAsync();
    }


    public async override Task<Tenant> InsertAsync(Tenant entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
      CheckAndSetId(entity);

      var dbContext = await GetDbContextAsync();

      var savedEntity = (await dbContext.Set<Tenant>().AddAsync(entity, GetCancellationToken(cancellationToken))).Entity;

      if (autoSave)
      {
        await dbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
      }

      return savedEntity;
    }
    public override Task<Tenant> FindByNameAsync(string name, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
      return base.FindByNameAsync(name, includeDetails, cancellationToken);
    }

    [Obsolete("Use FindByNameAsync method.")]
    public override Tenant FindByName(string name, bool includeDetails = true)
    {
      return FindByNameAsync(name, includeDetails).GetAwaiter().GetResult();
    }
  }
}
