using Common.Module.Constants;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.DomainServices;

public record MicroserviceCfg(string Name, string Path, string SourceUrl);
public record UiModuleCfg(string PluginName, string Expose, UiModuleLoadLevel LoadLevel, string Path, string SourceUrl, bool UseInAdmin = false);

public class DefaultApplicationsDomainService : DomainService
{
  private readonly string eleonsoftUiSourceUrl;
  private readonly string eleoncoreUiSourceUrl;

  private readonly List<MicroserviceCfg> _microservices = new List<MicroserviceCfg>();
  private readonly List<UiModuleCfg> _uiModules = new List<UiModuleCfg>();

  public DefaultApplicationsDomainService(IConfiguration configuration)
  {
    eleonsoftUiSourceUrl = configuration["App:UiSourceUrl"];
    eleoncoreUiSourceUrl = configuration["App:CoreUiSourceUrl"];
    _microservices = configuration.GetSection("Microservices").Get<List<MicroserviceCfg>>() ?? [];
    _uiModules = configuration.GetSection("UiModules").Get<List<UiModuleCfg>>() ?? [];
  }

  private readonly static Guid AppAdmin = Guid.Parse("00000000-0000-0000-0000-000000000011");

  private readonly static Guid Primeng = Guid.Parse("00000000-0000-0000-0000-000000000021");
  private readonly static Guid Sakaing = Guid.Parse("00000000-0000-0000-0000-000000000022");

  public ApplicationModuleEntity GetLayout(ClientApplicationStyleType styleType)
  {
    if (styleType == ClientApplicationStyleType.None)
    {
      return null;
    }
    if (styleType == ClientApplicationStyleType.PrimeNg)
    {
      return GetApplicationModulesFromConstants().First(m => m.Id == Primeng);
    }
    if (styleType == ClientApplicationStyleType.SakaiNg)
    {
      return GetApplicationModulesFromConstants().First(m => m.Id == Sakaing);
    }

    throw new NotImplementedException();
  }

  public ApplicationEntity GetDefaultApp()
  {
    return GetConstantApps().First(a => a.Id == AppAdmin);
  }

  public List<ApplicationModuleEntity> GetApplicationModulesFromConstants()
  {
    var constModules = new List<ApplicationModuleEntity>
    {
        new ApplicationModuleEntity(Primeng)
        {
            PluginName = "Primeng Layout",
            Url = "/modules/layout",
            Expose = "./Module",
            LoadLevel = UiModuleLoadLevel.RootModule,
            OrderIndex = 0,
        },
        new ApplicationModuleEntity(Sakaing)
        {
            PluginName = "SakaiNgLayout",
            Url = "/modules/sakai-ng-layout",
            Expose = "./SakaiNgLayout",
            LoadLevel = UiModuleLoadLevel.RootModule,
            OrderIndex = 0
        },
    };

    constModules.AddRange(_uiModules.Select(x => new ApplicationModuleEntity(GuidGenerator.Create())
    {
      PluginName = x.PluginName,
      Url = x.Path,
      Expose = x.Expose,
      LoadLevel = x.LoadLevel,
      OrderIndex = 0,
    }));
    return constModules;
  }

  public List<ModuleEntity> GetConstantModules()
  {
    var modules = new List<ModuleEntity>()
            {
                new ModuleEntity(Primeng)
                {
                    IsEnabled = true,
                    DisplayName = "Primeng Layout",
                    Type = ModuleType.Client,
                    Path = "/modules/layout",
                    Source = $"{eleonsoftUiSourceUrl}/primeng-layout",
                    IsSystem = true,
                    IsHidden = true,
                },
                new ModuleEntity(Sakaing)
                {
                    IsEnabled = true,
                    DisplayName = "Sakai Layout",
                    Type = ModuleType.Client,
                    Path = "/modules/sakai-ng-layout",
                    Source = $"{eleonsoftUiSourceUrl}/sakai-ng-layout",
                    IsSystem = true,
                    IsHidden = true,
                },
            };

    modules.AddRange(_uiModules.Select(x => new ModuleEntity(GuidGenerator.Create())
    {
      IsEnabled = true,
      DisplayName = x.PluginName,
      Type = ModuleType.Client,
      Path = x.Path,
      Source = x.SourceUrl,
      IsSystem = true,
    }));

    modules.AddRange(_microservices.Select(x => new ModuleEntity(GuidGenerator.Create())
    {
      IsEnabled = true,
      DisplayName = x.Name,
      Type = ModuleType.Server,
      Path = x.Path,
      Source = x.SourceUrl,
      IsSystem = true,
      IsHidden = false,
    }));
    return modules;
  }

  public List<ApplicationEntity> GetConstantApps()
  {
    var order = 0;
    return new List<ApplicationEntity>()
            {
                new ApplicationEntity(AppAdmin)
                {
                    Name = "Admin",
                    Path = "/apps/admin",
                    IsEnabled = true,
                    FrameworkType = ClientApplicationFrameworkType.Angular,
                    StyleType = ClientApplicationStyleType.PrimeNg,
                    ClientApplicationType = ClientApplicationType.Portal,
                    IsDefault = false,
                    IsSystem = true,
                    IsAuthenticationRequired = true,
                    RequiredPolicy = "Permission.Administration",
                    Modules = _uiModules
                      .Where(x => x.UseInAdmin)
                      .Select(module => new ApplicationModuleEntity(GuidGenerator.Create())
                      {
                        PluginName = module.PluginName,
                        Url = module.Path,
                        Expose = module.Expose,
                        LoadLevel = module.LoadLevel,
                        OrderIndex = order++,
                      })
                      .ToList(),
                    Properties = new List<ApplicationPropertyEntity>()
                    {
                        new ApplicationPropertyEntity()
                        {
                           Key = "UserSetting",
                           Value = "true"
                        },
                        new ApplicationPropertyEntity()
                        {
                            Key = "IsPwa",
                            Value = "true",
                        },
                        new ApplicationPropertyEntity()
                        {
                            Key = "PwaManifest",
                            Value = DefaultConfigurationHelper.PwaManifest,
                        },
                        new ApplicationPropertyEntity()
                        {
                            Key = "UseServiceWorker",
                            Value = "true",
                        },
                        new ApplicationPropertyEntity()
                        {
                            Key = "ServiceWorkerConfig",
                            Value = DefaultConfigurationHelper.ServiceWorkerConfig,
                        },
                    },
                },
            };
  }

  public class DefaultConfigurationHelper
  {
    public static string PwaManifest = @"{
   ""name"": ""Administration"",
   ""short_name"": ""Administration"",
   ""theme_color"": ""#1976d2"",
   ""background_color"": ""#fafafa"",
   ""display"": ""standalone"",
   ""scope"": ""/apps/admin"",
   ""start_url"": ""/apps/admin"",
   ""icons"": [
      {
         ""src"": ""/resources/pwa/logo_72x72.png"",
         ""sizes"": ""72x72"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_96x96.png"",
         ""sizes"": ""96x96"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_128x128.png"",
         ""sizes"": ""128x128"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_144x144.png"",
         ""sizes"": ""144x144"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_152x152.png"",
         ""sizes"": ""152x152"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_192x192.png"",
         ""sizes"": ""192x192"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_384x384.png"",
         ""sizes"": ""384x384"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      },
      {
         ""src"": ""/resources/pwa/logo_512x512.png"",
         ""sizes"": ""512x512"",
         ""type"": ""image/png"",
         ""purpose"": ""maskable any""
      }
   ]
}";
    public static string ServiceWorkerConfig = @"{
  ""$schema"": ""./node_modules/@angular/service-worker/config/schema.json"",
  ""index"": ""/index.html"",
  ""assetGroups"": [
    {
      ""name"": ""app"",
      ""installMode"": ""prefetch"",
      ""resources"": {
        ""files"": [
          ""/favicon.ico"",
          ""/index.html"",
          ""/manifest.webmanifest"",
          ""/*.css"",
          ""/*.js""
        ]
      }
    },
    {
      ""name"": ""assets"",
      ""installMode"": ""lazy"",
      ""updateMode"": ""prefetch"",
      ""resources"": {
        ""files"": [
          ""/*.mjs"",
          ""./assets/**"",
          ""/*.(svg|cur|jpg|jpeg|png|apng|webp|avif|gif|otf|ttf|woff|woff2)""
        ]
      }
    }
  ]
}";
  }
}


