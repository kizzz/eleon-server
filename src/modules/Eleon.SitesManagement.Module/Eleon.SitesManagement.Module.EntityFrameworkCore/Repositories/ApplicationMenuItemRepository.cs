using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.EntityFrameworkCore;

namespace VPortal.SitesManagement.Module.Repositories
{
  public class ApplicationMenuItemRepository : EfCoreRepository<SitesManagementDbContext, ApplicationMenuItemEntity, Guid>, IApplicationMenuItemRepository
  {
    private readonly IVportalLogger<ApplicationMenuItemRepository> logger;

    public ApplicationMenuItemRepository(
        IDbContextProvider<SitesManagementDbContext> dbContextProvider,
        IVportalLogger<ApplicationMenuItemRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<ApplicationMenuItemEntity>> GetListByApplicationId(Guid applicationId)
    {
      List<ApplicationMenuItemEntity> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.ClientApplicationMenuItems.Where(x => x.ApplicationId == applicationId).ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ApplicationMenuItemEntity>> GetListByApplicationIdAndMenuType(Guid applicationId, MenuType menuType)
    {
      List<ApplicationMenuItemEntity> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.ClientApplicationMenuItems
            //.AsNoTracking()
            .Where(x => x.ApplicationId == applicationId)
            .WhereIf(menuType == MenuType.General, x => x.MenuType == MenuType.General)
            .WhereIf(menuType == MenuType.User, x => x.MenuType == MenuType.User)
            .WhereIf(menuType == MenuType.Top, x => x.MenuType == MenuType.Top)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}


