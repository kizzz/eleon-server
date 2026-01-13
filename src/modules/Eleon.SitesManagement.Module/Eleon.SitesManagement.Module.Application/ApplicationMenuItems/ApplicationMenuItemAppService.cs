using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.ApplicationMenuItems
{
  public class ApplicationMenuItemAppService : SitesManagementAppService, IApplicationMenuItemAppService
  {
    private readonly ApplicationMenuItemDomainService domainService;
    private readonly IVportalLogger<ApplicationMenuItemAppService> logger;

    public ApplicationMenuItemAppService(
        ApplicationMenuItemDomainService domainService,
        IVportalLogger<ApplicationMenuItemAppService> logger)
    {
      this.domainService = domainService;
      this.logger = logger;
    }

    public async Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAsync(Guid applicationId)
    {
      List<ApplicationMenuItemDto> result = new();
      try
      {
        var entities = await domainService.GetListByApplicationId(applicationId);
        result = ObjectMapper.Map<List<ApplicationMenuItemEntity>, List<ApplicationMenuItemDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAndMenuTypeAsync(Guid applicationId, MenuType menuType)
    {
      List<ApplicationMenuItemDto> result = new();
      try
      {
        var entities = await domainService.GetListByApplicationIdAndMenuType(applicationId, menuType);
        result = ObjectMapper.Map<List<ApplicationMenuItemEntity>, List<ApplicationMenuItemDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<ApplicationMenuItemDto>> UpdateAsync(Guid applicationId, List<ApplicationMenuItemDto> itemsToUpdate)
    {
      List<ApplicationMenuItemDto> result = new();
      try
      {
        var mappedEntities = ObjectMapper.Map<List<ApplicationMenuItemDto>, List<ApplicationMenuItemEntity>>(itemsToUpdate);
        var entities = await domainService.Update(applicationId, mappedEntities);
        result = ObjectMapper.Map<List<ApplicationMenuItemEntity>, List<ApplicationMenuItemDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}


