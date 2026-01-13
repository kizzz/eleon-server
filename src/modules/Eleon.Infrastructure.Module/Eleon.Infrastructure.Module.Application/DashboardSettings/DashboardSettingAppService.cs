using Core.Infrastructure.Module.DomainServices;
using Core.Infrastructure.Module.Entities;
using Logging.Module;
using VPortal.Infrastructure.Module;


namespace Core.Infrastructure.Module.DashboardSettings
{
  public class DashboardSettingAppService : InfrastructureAppService, IDashboardSettingAppService
  {
    private readonly IVportalLogger<DashboardSettingAppService> logger;
    private readonly DashboardSettingDomainService domainService;

    public DashboardSettingAppService(
        IVportalLogger<DashboardSettingAppService> logger,
        DashboardSettingDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<List<DashboardSettingDto>> GetDashboardSettings()
    {
      List<DashboardSettingDto> result = new List<DashboardSettingDto>();
      try
      {
        var entities = await domainService.GetDashboardSettings();
        result = ObjectMapper.Map<List<DashboardSettingEntity>, List<DashboardSettingDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<string> UpdateSettings(List<DashboardSettingDto> dashboardSettingDtos, bool setAsDefault)
    {
      string result = string.Empty;
      try
      {
        var entities = ObjectMapper.Map<List<DashboardSettingDto>, List<DashboardSettingEntity>>(dashboardSettingDtos);
        result = await domainService.CreateOrUpdateSettings(entities, setAsDefault);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<string> DeleteDashboardSettings(Guid dashboardSettingEntityId)
    {
      string result = string.Empty;
      try
      {
        result = await domainService.DeleteDashboardSettings(dashboardSettingEntityId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
