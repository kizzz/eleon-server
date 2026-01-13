using EleonsoftSdk.modules.Azure;
using Volo.Abp.DependencyInjection;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
public class AzureNotificator
{
  private readonly NotificatorBaseHelperService _notificatorHelperService;

  public AzureNotificator(
      NotificatorBaseHelperService recipientResolver)
  {
    _notificatorHelperService = recipientResolver;
  }

  public async Task SendEmailAsync(AzureEwsOptions options, string subject, string body, List<string> targetEmails, bool isHtml = true, Dictionary<string, string> attachments = null)
  {
    ArgumentNullException.ThrowIfNull(options, nameof(options));

    targetEmails = _notificatorHelperService.FilterEmails(targetEmails);

    if (targetEmails.Count == 0)
    {
      return;
    }

    var azure = new AzureMailService(options);
    await azure.SendEmailAsync(targetEmails, _notificatorHelperService.GetValidatedSubjectOrDefault(subject), body, isHtml, attachments);
  }
}
