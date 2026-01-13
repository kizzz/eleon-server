using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Localization;
using VPortal.TenantManagement.Module.TenantHostname;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class TenantHostnameDomainService : DomainService, ITransientDependency
  {
    private readonly IVportalLogger<TenantHostnameDomainService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly TenantSettingDomainService tenantSettingDomainService;
    private readonly ITenantRepository tenantRepository;
    private readonly IStringLocalizer<TenantManagementResource> localizer;
    private readonly IDistributedEventBus eventBus;
    private readonly IConfiguration configuration;

    public TenantHostnameDomainService(
        IVportalLogger<TenantHostnameDomainService> logger,
        IServiceProvider serviceProvider,
        TenantSettingDomainService tenantSettingDomainService,
        ITenantRepository tenantRepository,
        IStringLocalizer<TenantManagementResource> localizer,
        IDistributedEventBus eventBus,
        IConfiguration configuration)
    {
      this.logger = logger;
      this.serviceProvider = serviceProvider;
      this.tenantSettingDomainService = tenantSettingDomainService;
      this.tenantRepository = tenantRepository;
      this.localizer = localizer;
      this.eventBus = eventBus;
      this.configuration = configuration;
    }

    public async Task UpdateInternalHostAndTenantsHostnamesAsync()
    {
      try
      {
        //var bindingCfg = HostnameBindingConfiguration.Parse(configuration.GetSection("Bindings"));
        var bindingCfg = new List<HostnameBindingConfiguration>() {
                    new HostnameBindingConfiguration {
                        Domain = configuration["App:Domain"],
                        Subdomain = string.Empty,
                        Port = 443,
                        IsSsl = true,
                        ResolveTenantBySubdomain = true
                    }
                };
        var tenants = await tenantRepository.GetListAsync();
        tenants.AddFirst(null);

        var configuredHostnames = tenants.SelectMany(x => GetConfiguredTenantHostnames(x?.Id, x?.Name, bindingCfg)).Concat([new TenantHostnameEntity(GuidGenerator.Create()) {
                    Domain = "localhost",
                    Internal = true,
                    Status = HostnameStatus.Active,
                    IsSsl = true,
                    Port = 443,
                }]).ToList();
        var hostnamesByTenant = configuredHostnames.GroupBy(x => x.TenantId).ToList();
        foreach (var hostnames in hostnamesByTenant)
        {
          await tenantSettingDomainService.ReplaceIntenalTenantHostnames(hostnames.Key, hostnames.ToList());
        }

        await SendUpdateApplicationUrlsMessageAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<TenantHostnameEntity>> GetTenantHostnames(Guid? tenantId)
    {
      List<TenantHostnameEntity> result = null;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
          result = settings.Hostnames;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<TenantHostnameEntity>> GetApplicationHostnames(Guid? tenantId, Guid? applicationId)
    {
      List<TenantHostnameEntity> result = null;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
          result = settings.Hostnames.Where(x => x.AppId == applicationId).ToList();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public List<TenantHostnameEntity> GetNonExistentTenantHostnames(string tenantName)
    {
      List<TenantHostnameEntity> result = null;
      try
      {
        var bindingCfg = HostnameBindingConfiguration.Parse(configuration.GetSection("Bindings"));
        result = GetConfiguredTenantHostnames(null, tenantName, bindingCfg);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    internal async Task<TenantHostnameEntity> AddNonInternalTenantHostname(
        Guid? tenantId,
        string domain,
        string subdomain,
        bool acceptsClientCertificate,
        bool isSsl,
        VportalApplicationType applicationType,
        bool isDefault,
        int port,
        Guid? appId,
        bool isInternal = false)
    {
      var hostname = new TenantHostnameEntity(GuidGenerator.Create())
      {
        Domain = domain,
        Subdomain = subdomain,
        IsSsl = isSsl,
        Port = port,
        ApplicationType = applicationType,
        Internal = isInternal,
        TenantId = tenantId,
        AcceptsClientCertificate = acceptsClientCertificate,
        Default = isDefault,
        AppId = appId,
      };

      await EnsureHostnameUnique(hostname);

      {
        var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
        settings.Hostnames.Add(hostname);
        await tenantSettingDomainService.UpdateSettings(settings);
      }

      await SendUpdateApplicationUrlsMessageAsync();

      return hostname;
    }

    internal async Task<TenantHostnameEntity> RemoveNonInternalTenantHostname(Guid? tenantId, Guid? hostnameId)
    {
      TenantHostnameEntity hostnameResult = null;
      {
        var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
        hostnameResult = settings.Hostnames.First(x => x.Id == hostnameId);
        //if (hostnameResult.Internal)
        //{
        //    throw new Exception("Can not remove internal hostname.");
        //}

        settings.Hostnames.Remove(hostnameResult);
        await tenantSettingDomainService.UpdateSettings(settings);
      }

      //if (CurrentTenant.Id != null)
      //{
      //    using (CurrentTenant.Change(Guid.Empty))
      //    {
      //        var settings = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      //        hostnameResult = settings.Hostnames.First(x => x.Id == hostnameId);
      //        if (hostnameResult.Internal)
      //        {
      //            throw new Exception("Can not remove internal hostname.");
      //        }

      //        settings.Hostnames.Remove(hostnameResult);
      //        await tenantSettingDomainService.UpdateSettings(settings);
      //    }
      //}

      return hostnameResult;
    }

    private async Task EnsureHostnameUnique(TenantHostnameEntity newHostname)
    {
      var settings = await tenantSettingDomainService.GetAllSettings();
      var existing = settings.SelectMany(x => x.Hostnames.Select(h => h.HostnameWithPort));

      bool isUnique = existing.All(x => x != newHostname.HostnameWithPort);

      if (!isUnique)
      {
        throw new AbpValidationException(localizer["CreateIisBinding:Error:HostnameIsTaken"]);
      }
    }

    private async Task SendUpdateApplicationUrlsMessageAsync()
    {
      var settings = await tenantSettingDomainService.GetAllSettings();
      var hostnames = settings.SelectMany(x => x.Hostnames).ToList();
      var urlsByApp = hostnames
          .GroupBy(x => x.ApplicationType)
          .ToDictionary(x => x.Key, g => g.Select(h => h.Url).ToList());

      await eventBus.PublishAsync(new ApplicationUrlsChangedMsg()
      {
        Urls = urlsByApp.Select(x => new ApplicationUrlsEto()
        {
          ApplicationType = x.Key,
          ApplicationUrls = x.Value,
        }).ToList(),
      });
    }

    private List<TenantHostnameEntity> GetConfiguredTenantHostnames(Guid? tenantId, string tenantName, List<HostnameBindingConfiguration> bindings)
    {
      var tenantBindings = bindings
          .WhereIf(tenantName.NonEmpty(), x => x.ResolveTenantBySubdomain);

      var result = new List<TenantHostnameEntity>();
      foreach (var binding in tenantBindings)
      {
        foreach (var secure in new bool[] { false }) // important: true was removed to removed subdomains with secure-
        {
          string subdomain = binding.GetSubdomain(tenantName, secure);

          var hostname = new TenantHostnameEntity(GuidGenerator.Create())
          {
            Subdomain = subdomain,
            Domain = binding.Domain,
            ApplicationType = VportalApplicationType.Undefined,
            Port = binding.Port,
            TenantId = tenantId,
            IsSsl = binding.IsSsl,
            Internal = true,
            AcceptsClientCertificate = secure,
          };

          result.Add(hostname);
        }
      }

      return result;
    }
  }
}
