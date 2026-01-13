using Volo.Abp.Domain.Repositories;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.Repositories
{
  public interface IApplicationMenuItemRepository : IBasicRepository<ApplicationMenuItemEntity, Guid>
  {
    Task<List<ApplicationMenuItemEntity>> GetListByApplicationId(Guid applicationId);
    Task<List<ApplicationMenuItemEntity>> GetListByApplicationIdAndMenuType(Guid applicationId, MenuType menuType);
  }
}


