using EleonsoftModuleCollector.Commons.Module.BasicNotificators.Helpers;
using EleonsoftSdk.modules.Azure;
using Messaging.Module.Messages;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating.Razor;
using Volo.Abp.TextTemplating.Scriban;
using VPortal.MassTransit.RabbitMQ;
using VPortal.Notificator.Module.EventServices;
using VPortal.Notificator.Module.Jobs;

namespace VPortal.Notificator.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ModuleDomainSharedModule),
    typeof(AbpTextTemplatingRazorModule),
    typeof(AbpTextTemplatingScribanModule)
)]
public class NotificatorDomainModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddBasicNotificatorServices();
  }
}
