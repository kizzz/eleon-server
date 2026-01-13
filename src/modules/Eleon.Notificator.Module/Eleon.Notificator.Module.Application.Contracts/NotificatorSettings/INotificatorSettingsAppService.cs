using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
using Volo.Abp.Application.Services;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails
{
  public interface INotificatorSettingsAppService : IApplicationService
  {
    Task<NotificatorSettingsDto> GetAsync();
    Task UpdateAsync(NotificatorSettingsDto input);
    Task<string> SendCustomTestEmailAsync(SendTestEmailInputDto input);
    Task<List<string>> ValidateTemplateAsync(string templateType, string template);
  }
}
