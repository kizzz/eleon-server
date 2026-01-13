using Commons.Module.Constants.Permission;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;
using VPortal.Notificator.Module;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Emails
{
  [Authorize(DefaultPermissions.TenantSettingsManagement)]
  public class NotificatorSettingsAppService : NotificatorModuleAppService, INotificatorSettingsAppService
  {
    private readonly NotificatorSettingsDomainService domainService;
    private readonly IVportalLogger<NotificatorSettingsAppService> logger;

    public NotificatorSettingsAppService(
        NotificatorSettingsDomainService domainService,
        IVportalLogger<NotificatorSettingsAppService> logger)
    {
      this.domainService = domainService;
      this.logger = logger;
    }

    public async Task<NotificatorSettingsDto> GetAsync()
    {

      try
      {
        var settings = await domainService.GetAsync();
        return ObjectMapper.Map<NotificatorSettings, NotificatorSettingsDto>(settings);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public Task UpdateAsync(NotificatorSettingsDto input)
    {

      try
      {
        var settings = ObjectMapper.Map<NotificatorSettingsDto, NotificatorSettings>(input);
        return domainService.UpdateAsync(settings);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<string> SendCustomTestEmailAsync(SendTestEmailInputDto input)
    {
      string reply = string.Empty;
      try
      {
        reply = await domainService.SendCustomTestEmailAsync(input.Subject, input.SenderEmailAddress, input.TargetEmailAddress, input.Body);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return reply;
    }

    public async Task<List<string>> ValidateTemplateAsync(string templateType, string template)
    {

      try
      {
        return await domainService.ValidateTemplateAsync(templateType, template);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
