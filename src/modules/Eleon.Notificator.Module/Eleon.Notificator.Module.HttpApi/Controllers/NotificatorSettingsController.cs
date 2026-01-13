using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
using Volo.Abp;
using VPortal.Notificator.Module;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/NotificatorSettings")]
  public class NotificatorSettingsController : NotificatorModuleController, INotificatorSettingsAppService
  {
    private readonly IVportalLogger<NotificatorSettingsController> logger;
    private readonly INotificatorSettingsAppService emailAppService;

    public NotificatorSettingsController(
        IVportalLogger<NotificatorSettingsController> logger,
        INotificatorSettingsAppService emailAppService)
    {
      this.logger = logger;
      this.emailAppService = emailAppService;
    }

    [HttpGet("Get")]
    public async Task<NotificatorSettingsDto> GetAsync()
    {

      var response = await emailAppService.GetAsync();


      return response;
    }

    [HttpPost("SendCustomTestEmail")]
    public async Task<string> SendCustomTestEmailAsync(SendTestEmailInputDto input)
    {

      var response = await emailAppService.SendCustomTestEmailAsync(input);


      return response;
    }

    [HttpPost("Update")]
    public async Task UpdateAsync(NotificatorSettingsDto input)
    {

      await emailAppService.UpdateAsync(input);

    }

    [HttpPost("ValidateTemplate")]
    public async Task<List<string>> ValidateTemplateAsync(string templateType, string template)
    {

      var response = await emailAppService.ValidateTemplateAsync(templateType, template);


      return response;
    }
  }
}
