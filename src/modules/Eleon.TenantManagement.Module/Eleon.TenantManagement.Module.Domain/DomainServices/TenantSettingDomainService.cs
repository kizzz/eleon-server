using Common.Module.Constants;
using Common.Module.Helpers;
using EleonsoftModuleCollector.Commons.Module.Constants;
using EleonsoftModuleCollector.Commons.Module.Messages;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.SystemHealth;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.SettingManagement;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;
using VPortal.TenantManagement.Module.Settings;

namespace VPortal.TenantManagement.Module.DomainServices
{
    
    public class TenantSettingDomainService : DomainService
    {
        private readonly IVportalLogger<TenantSettingDomainService> logger;
        private readonly IDistributedEventBus _eventBus;
        private readonly IConfiguration configuration;
        private readonly ITenantSettingRepository tenantSettingRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly TenantSettingsCacheService tenantSettingsCache;
        private readonly ISettingManager _settingManager;

        public TenantSettingDomainService(
            IVportalLogger<TenantSettingDomainService> logger,
            IDistributedEventBus massTransitPublisher,
            IConfiguration configuration,
            ITenantSettingRepository tenantSettingRepository,
            IServiceProvider serviceProvider,
            TenantSettingsCacheService tenantSettingsCache,
            ISettingManager settingManager)
        {
            this.logger = logger;
            this._eventBus = massTransitPublisher;
            this.configuration = configuration;
            this.tenantSettingRepository = tenantSettingRepository;
            this.serviceProvider = serviceProvider;
            this.tenantSettingsCache = tenantSettingsCache;
            _settingManager = settingManager;
        }

        public async Task<TenantSettingEntity> GetOrCreateTenantSettings(Guid? tenantId)
        {
            TenantSettingEntity result = null;
            try
            {
                var setting = await tenantSettingRepository.GetByTenantId(tenantId);
                if (setting == null)
                {
                    setting = new TenantSettingEntity(GuidGenerator.Create(), tenantId);
                    setting.AppearanceSettings = new TenantAppearanceSettingEntity(GuidGenerator.Create());
                    await tenantSettingRepository.InsertAsync(setting, true);
                }

                DecryptData(setting);
                result = setting;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }

        public async Task<List<TenantSettingEntity>> GetAllSettings()
        {
            List<TenantSettingEntity> result = null;
            try
            {
                var settings = await tenantSettingRepository.GetListAsync(true);
                settings.ForEach(DecryptData);
                result = settings.ToList();
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }

        public async Task<string> GetExternalLoginAdminIdentifier(Guid? tenantId, ExternalLoginProviderType providerType)
        {
            string result = null;
            try
            {
                var settings = await tenantSettingRepository.GetByTenantId(tenantId);
                result = settings.ExternalProviders.FirstOrDefault(x => x.Type == providerType)?.AdminIdentifier;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }

        public async Task SetExternalProviderSettings(Guid? tenantId, List<TenantExternalLoginProviderEntity> providerSettings)
        {
            try
            {
                await SetTenantExternalProviderSettings(tenantId, providerSettings);
                //if (CurrentTenant.Id == null)
                //{
                //    return;
                //}
                //using (CurrentTenant.Change(Guid.Empty))
                //{
                //    await SetTenantExternalProviderSettings(tenantId, providerSettings);
                //}
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

        }

        private async Task SetTenantExternalProviderSettings(Guid? tenantId, List<TenantExternalLoginProviderEntity> providerSettings)
        {
            bool hasMoreThanOneProvider = providerSettings.Count(x => x.Enabled) > 1;
            if (hasMoreThanOneProvider)
            {
                throw new Exception("You can only enable one external provider at a time.");
            }

            var tenantSettings = await GetOrCreateTenantSettings(tenantId);
            foreach (var providerSetting in providerSettings)
            {
                var provider = tenantSettings.ExternalProviders.FirstOrDefault(x => x.Type == providerSetting.Type);
                if (provider == null)
                {
                    provider = new TenantExternalLoginProviderEntity(GuidGenerator.Create())
                    {
                        Authority = providerSetting.Authority,
                        ClientId = providerSetting.ClientId,
                        ClientSecret = providerSetting.ClientSecret,
                        Enabled = providerSetting.Enabled,
                        Type = providerSetting.Type,
                        AdminIdentifier = providerSetting.AdminIdentifier,
                    };

                    tenantSettings.ExternalProviders.Add(provider);
                }
                else
                {
                    provider.Authority = providerSetting.Authority;
                    provider.ClientId = providerSetting.ClientId;
                    provider.ClientSecret = providerSetting.ClientSecret;
                    provider.Enabled = providerSetting.Enabled;
                    provider.AdminIdentifier = providerSetting.AdminIdentifier;
                }
            }

            await UpdateSettings(tenantSettings);
        }

        internal async Task UpdateSettings(TenantSettingEntity settings)
        {
            EncryptData(settings);
            await tenantSettingRepository.UpdateAsync(settings, true);
            await tenantSettingsCache.UpdateCache();
        }

        internal async Task ReplaceIntenalTenantHostnames(Guid? tenantId, List<TenantHostnameEntity> updated)
        {
            {
                var setting = await GetOrCreateTenantSettings(tenantId);

                foreach (var hostname in updated)
                {
                    var existing = setting.Hostnames.FirstOrDefault(x => x.Url == hostname.Url);

                    if (existing == null)
                    {
                        setting.Hostnames.Add(hostname);
                    }
                }

                //setting.Hostnames.RemoveAll(x => x.Internal);
                //setting.Hostnames.AddRange(updated);
                EncryptData(setting);
                await tenantSettingRepository.UpdateAsync(setting, true);
            }
            //if (CurrentTenant.Id == null)
            //{
            //    return;
            //}
            //using (CurrentTenant.Change(Guid.Empty))
            //{
            //    var setting = await GetOrCreateTenantSettings(tenantId);
            //    setting.Hostnames.RemoveAll(x => x.Internal);
            //    setting.Hostnames.AddRange(updated);
            //    EncryptData(setting);
            //    await tenantSettingRepository.UpdateAsync(setting, true);
            //}
        }

        private void EncryptData(TenantSettingEntity setting)
        {
            foreach (var provider in setting.ExternalProviders)
            {
                var data = new EncryptedProviderData()
                {
                    ClientSecret = provider.ClientSecret,
                    ClientId = provider.ClientId,
                    Authority = provider.Authority,
                };

                provider.Data = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(data));
                provider.ClientSecret = null;
                provider.ClientId = null;
                provider.Authority = null;
            }
        }

        private void DecryptData(TenantSettingEntity setting)
        {
            foreach (var provider in setting.ExternalProviders)
            {
                if (provider.Data.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var decryptedJson = EncryptionHelper.Decrypt(provider.Data);
                var data = JsonConvert.DeserializeObject<EncryptedProviderData>(decryptedJson);
                provider.ClientSecret = data.ClientSecret;
                provider.ClientId = data.ClientId;
                provider.Authority = data.Authority;
                provider.Data = null;
            }
        }

        public async Task<TenantSystemHealthSettings> GetTenantSystemHealthSettings()
        {
            try
            {
                var telemetrySettingsValueObject = await _settingManager.GetOrDefaultForCurrentTenantAsync<TenantSystemHealthSettings>(TenantManagementSettings.TenantSystemHealthSettings);
                return telemetrySettingsValueObject;
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

        public async Task<bool> UpdateTenantSystemHealthSettingsAsync(TenantSystemHealthSettings settings)
        {
            try
            {
                await _settingManager.SetForCurrentTenantAsync<TenantSystemHealthSettings>(TenantManagementSettings.TenantSystemHealthSettings, settings);
                await _eventBus.PublishAsync(new SystemHealthSettingsUpdatedMsg { Telemetry = settings.Telemetry.ToOptions() });
                await _eventBus.PublishAsync(new SendToClientMsg
                {
                    IsToAll = true,
                    Method = DefaultSystemHubMethods.UpdateAppConfig,
                    Data = settings,
                });
                return true;
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

        private class EncryptedProviderData
        {
            public string Authority { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
