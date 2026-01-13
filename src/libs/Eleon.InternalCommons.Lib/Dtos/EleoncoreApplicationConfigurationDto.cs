using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using Volo.Abp.Data;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration
{
  public class EleoncoreApplicationConfigurationDto
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

    public ClientApplicationDto ClientApplication { get; set; }
    public List<ApplicationModuleDto> Modules { get; set; }

    public OAuthConfigDto OAuthConfig { get; set; }
    public WebPushConfigDto WebPush { get; set; }

  }
  public class OAuthConfigDto
  {
    public string ClientId { get; set; }
    public string ResponseType { get; set; }
    public string Scope { get; set; }
    public bool UseSilentRefresh { get; set; } = false;
  }

  public class WebPushConfigDto
  {
    public string PublicKey { get; set; }
  }
}


