using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.Consts;

namespace VPortal.SitesManagement.Module.ApplicationMenuItems
{
  public interface IApplicationMenuItemAppService : IApplicationService
  {
    Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAsync(Guid applicationId);
    Task<List<ApplicationMenuItemDto>> UpdateAsync(Guid applicationId, List<ApplicationMenuItemDto> itemsToUpdate);
    Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAndMenuTypeAsync(Guid applicationId, MenuType menuType);
  }
}


