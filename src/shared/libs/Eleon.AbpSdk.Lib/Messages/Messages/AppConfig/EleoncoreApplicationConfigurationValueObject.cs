using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations.ObjectExtending;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using Volo.Abp.Data;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects
{
  public class EleoncoreApplicationConfigurationValueObject
  {
    public ApplicationLocalizationConfigurationDto Localization { get; set; }
    public ApplicationAuthConfigurationDto Auth { get; set; }
    public CurrentUserDto CurrentUser { get; set; }
    public ApplicationFeatureConfigurationDto Features { get; set; }
    public CurrentTenantDto CurrentTenant { get; set; }
    public ExtraPropertyDictionary ExtraProperties { get; set; }
    public bool Production { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationPath { get; set; }
    public string CorePath { get; set; }
    public string AuthPath { get; set; }
    public string FrameworkType { get; set; }
    public string StyleType { get; set; }
    public string ClientApplicationType { get; set; }
    public ClientApplicationEto ClientApplication { get; set; }
    public List<ApplicationModuleEto> Modules { get; set; }
    public OAuthConfigValueObject OAuthConfig { get; set; }
    public WebPushConfigValueObject WebPush { get; set; }

  }
  public class OAuthConfigValueObject
  {
    public string ClientId { get; set; }
    public string ResponseType { get; set; }
    public string Scope { get; set; }
    public bool UseSilentRefresh { get; set; } = false;
  }

  public class WebPushConfigValueObject
  {
    public string PublicKey { get; set; }
  }
}
