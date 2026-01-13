using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.SystemMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ModuleCollector.Commons.Module.Proxy.Constants;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Localization;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.SitesManagement.Module.Repositories;

namespace VPortal.SitesManagement.Module.DomainServices
{
    public class ClientApplicationDomainService : DomainService
    {
        private readonly DefaultApplicationsDomainService defaultApplicationsDomainService;
        private readonly IClientApplicationRepository _clientApplicationRepository;
        private readonly IConfiguration _configuration;
        private readonly IVportalLogger<ClientApplicationDomainService> _logger;
        private readonly ICurrentTenant currentTenant;
        private readonly IStringLocalizer<SitesManagementResource> localizer;
        private readonly IDistributedEventBus eventBus;

        public ClientApplicationDomainService(
            DefaultApplicationsDomainService defaultApplicationsDomainService,
            IClientApplicationRepository clientApplicationRepository,
            IConfiguration configuration,
            IVportalLogger<ClientApplicationDomainService> logger,
            ICurrentTenant currentTenant,
            IStringLocalizer<SitesManagementResource> localizer,
            IDistributedEventBus eventBus)
        {
            this.defaultApplicationsDomainService = defaultApplicationsDomainService;
            _clientApplicationRepository = clientApplicationRepository;
            _configuration = configuration;
            _logger = logger;
            this.currentTenant = currentTenant;
            this.localizer = localizer;
            this.eventBus = eventBus;
        }

        public async Task<ApplicationEntity> GetAsync(Guid id)
        {
            ApplicationEntity result = null;
            try
            {
                result = GetConstantApps().FirstOrDefault(a => a.Id == id) ?? await _clientApplicationRepository.GetAsync(id);
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<List<ApplicationEntity>> GetAllAsync()
        {
            List<ApplicationEntity> result = null;
            try
            {
                result = await _clientApplicationRepository.GetListAsync(true);
                result.AddRange(GetConstantApps());
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<ApplicationEntity> CreateAsync(string name, string path, string source, bool isEnabled, bool isAuthenticationRequired, ClientApplicationFrameworkType frameworkType,
                                                               ClientApplicationStyleType styleType, ClientApplicationType clientApplicationType, string icon = null,bool useDedicatedDb = false, Guid? parentId = null, ApplicationType appType = ApplicationType.Application, List<ApplicationPropertyEntity> properties = null)
        {
            ApplicationEntity result = null;
            try
            {
                var clientApplication = new ApplicationEntity(GuidGenerator.Create())
                {
                    Name = name.Trim(),
                    Source = source.Trim(),
                    IsEnabled = isEnabled,
                    FrameworkType = frameworkType,
                    Path = path.Trim(),
                    IsAuthenticationRequired = isAuthenticationRequired,
                    StyleType = styleType,
                    ClientApplicationType = clientApplicationType,
                    Icon = icon,
                    UseDedicatedDatabase = useDedicatedDb,
                    ParentId = parentId,
                    AppType = appType,
                    Properties = properties ?? []
                };

                var apps = (await _clientApplicationRepository.GetDbSetAsync()).Where(app => app.TenantId == currentTenant.Id);
                var existApp = await apps.Where(app => app.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
                if (existApp != null) throw new AbpValidationException([new ValidationResult("SitesManagement::Applications:AlreadyExists:WithName", [nameof(ApplicationEntity.Name)])]);
                existApp = await apps.Where(app => app.Path.ToLower() == path.ToLower()).FirstOrDefaultAsync();
                if (existApp != null) throw new AbpValidationException([new ValidationResult("SitesManagement::Applications:AlreadyExists:WithPath", [nameof(ApplicationEntity.Path)])]);

                ValidateApplicationStyleType(clientApplication);

                await _clientApplicationRepository.InsertAsync(clientApplication, true);
                await PublishApplicationUpdated();
                result = clientApplication;
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<ApplicationEntity> UpdateAsync(ApplicationEntity clientApplication, string name, string path, string source, bool isEnabled, bool isAuthenticationRequired,
                                                               ClientApplicationFrameworkType frameworkType, ClientApplicationStyleType styleType,
                                                               ClientApplicationType clientApplicationType, bool isDefault, string icon = null, bool useDedicatedDb = false, List<ApplicationPropertyEntity> properties = null)
        {
            ApplicationEntity result = null;
            try
            {
                if (clientApplication.IsSystem)
                {
                    throw new UserFriendlyException(localizer["ConstantApplication:Update:Forbidden"]);
                }

                var apps = (await _clientApplicationRepository.GetDbSetAsync())
                    .Where(app => app.TenantId == currentTenant.Id)
                    .Where(app => app.Id != clientApplication.Id);
                var existApp = await apps.Where(app => app.Name == name).FirstOrDefaultAsync();
                if (existApp != null) throw new AbpValidationException([new ValidationResult("SitesManagement::Applications:AlreadyExists:WithName", [nameof(ApplicationEntity.Name)])]);
                existApp = await apps.Where(app => app.Path == path).FirstOrDefaultAsync();
                if (existApp != null) throw new AbpValidationException([new ValidationResult("SitesManagement::Applications:AlreadyExists:WithPath", [nameof(ApplicationEntity.Path)])]);
                
                clientApplication.Name = name;
                clientApplication.Path = path;
                clientApplication.Source = source;
                clientApplication.IsEnabled = isEnabled;
                clientApplication.FrameworkType = frameworkType;
                clientApplication.IsAuthenticationRequired = isAuthenticationRequired;
                clientApplication.StyleType = styleType;
                clientApplication.ClientApplicationType = clientApplicationType;
                clientApplication.Icon = icon;
                clientApplication.UseDedicatedDatabase = useDedicatedDb;
                
                if (properties != null)
                {
                    clientApplication.Properties = properties.Select(s => new ApplicationPropertyEntity(GuidGenerator.Create())
                    {
                        Key = s.Key,
                        Value = s.Value,
                    }).ToList();
                }

                ValidateApplicationStyleType(clientApplication);

                await _clientApplicationRepository.UpdateAsync(clientApplication);
                await SetDefaultApplicationAsync(clientApplication, isDefault);

                await PublishApplicationUpdated();
                result = clientApplication;
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                foreach (var module in GetConstantApps())
                {
                    if (module.Id == id)
                    {
                        throw new UserFriendlyException(localizer["ConstantApplication:Delete:Forbidden"]);
                    }
                }

                await _clientApplicationRepository.DeleteAsync(id);
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
        }

        public async Task<List<ApplicationEntity>> GetByTenantIdAsync(Guid? tenantId)
        {
            List<ApplicationEntity> result = null;
            try
            {
                result = await (await _clientApplicationRepository.GetDbSetAsync())
                    .Where(app => app.TenantId == tenantId)
                    .ToListAsync();

                result.AddRange(GetConstantApps());
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<List<ApplicationEntity>> GetEnabledApplicationsAsync()
        {
            List<ApplicationEntity> result = null;
            try
            {
                result = await (await _clientApplicationRepository.GetDbSetAsync())
                    .Include(app => app.Modules)
                    .Include(app => app.Properties)
                    .Where(app => app.IsEnabled)
                    .ToListAsync();

                result.AddRange(GetConstantApps().Where(a => a.IsEnabled));
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }


        public async Task<ApplicationEntity> RemoveModuleFromApplication(Guid applicationId, Guid moduleId)
        {
            ApplicationEntity result = null;
            try
            {
                var clientApplication = await GetAsync(applicationId);

                if (clientApplication.IsSystem)
                {
                    throw new UserFriendlyException(localizer["ConstantApplication:Update:Forbidden"]);
                }

                clientApplication.Modules = clientApplication.Modules.Where(m => m.Id != moduleId).ToList();

                await _clientApplicationRepository.UpdateAsync(clientApplication, true);
                await PublishApplicationUpdated();
                result = clientApplication;
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<bool> AddBulkModulesToApplication(List<ApplicationModuleEntity> applicationModuleEntities)
        {
            bool result = false;
            try
            {
                var childApps = new List<ApplicationEntity>();

                foreach (var appModule in applicationModuleEntities)
                {
                    var clientApplication = await GetAsync(appModule.ClientApplicationEntityId);

                    if (clientApplication.IsSystem)
                    {
                        throw new UserFriendlyException(localizer["ConstantApplication:Update:Forbidden"]);
                    }

                    //clientApplication.Modules.Clear();
                    clientApplication.Modules.Add(new ApplicationModuleEntity(GuidGenerator.Create())
                    {
                        LoadLevel = appModule.LoadLevel,
                        ClientApplicationEntityId = appModule.ClientApplicationEntityId,
                        OrderIndex = appModule.OrderIndex,
                        Expose = appModule.Expose,
                        PluginName = appModule.PluginName,
                        Url = appModule.Url,
                    });

                    await _clientApplicationRepository.UpdateAsync(clientApplication, true);

                    //var module = new ApplicationEntity(GuidGenerator.Create())
                    //{
                    //    Name = appModule.PluginName,
                    //    Source = appModule.Url,
                    //    IsEnabled = true,
                    //    FrameworkType = ClientApplicationFrameworkType.None,
                    //    Path = appModule.PluginName,
                    //    IsAuthenticationRequired = false,
                    //    StyleType = ClientApplicationStyleType.None,
                    //    ClientApplicationType = ClientApplicationType.Portal,
                    //    Icon = "",
                    //    UseDedicatedDatabase = false,
                    //    ParentId = clientApplication.Id,
                    //    AppType = ApplicationType.Module,
                    //    LoadLevel = appModule.LoadLevel,
                    //    OrderIndex = appModule.OrderIndex,
                    //    Expose = appModule.Expose,
                    //    Properties = []
                    //};

                    //childApps.Add(module);
                }

                //await _clientApplicationRepository.InsertManyAsync(childApps);

                await PublishApplicationUpdated();
                result = true;
            }
            catch (Exception e)
            {
                _logger.CaptureAndSuppress(e);
            }
            finally
            {
            }
            return result;
        }

        public async Task<ApplicationEntity> GetDefaultApplicationAsync()
        {

            try
            {
                var defaultApp = (await _clientApplicationRepository.GetDbSetAsync()).FirstOrDefault(app => app.IsDefault);
                
                if (defaultApp == null)
                {
                    defaultApp = defaultApplicationsDomainService.GetDefaultApp();
                }

                return defaultApp;
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }

            return null;
        }

        private async Task SetDefaultApplicationAsync(ApplicationEntity clientApplication, bool value)
        {
            try
            {
                if (clientApplication.IsDefault == value)
                {
                    return;
                }

                if (value)
                {
                    var apps = await (await _clientApplicationRepository.GetDbSetAsync()).Where(x => x.IsDefault).ToListAsync();
                    apps.ForEach(x => x.IsDefault = false);
                    await _clientApplicationRepository.UpdateManyAsync(apps);
                }

                clientApplication.IsDefault = value;
                await _clientApplicationRepository.UpdateAsync(clientApplication);
            }
            catch (Exception e)
            {
                _logger.Capture(e);
            }
            finally
            {
            }
        }
        private List<ApplicationEntity> GetConstantApps()
        {
            return defaultApplicationsDomainService.GetConstantApps();
        }

        private void ValidateApplicationStyleType(ApplicationEntity clientApplication)
        {
            bool isValid = true;
            if (clientApplication.StyleType == ClientApplicationStyleType.None)
            {
                return;
            }
            if (clientApplication.FrameworkType == ClientApplicationFrameworkType.Angular)
            {
                isValid = clientApplication.StyleType switch 
                { 
                    ClientApplicationStyleType.PrimeNg => true,
                    ClientApplicationStyleType.SakaiNg => true,
                    ClientApplicationStyleType.Bootstrap => true,
                    ClientApplicationStyleType.Material => true,
                    _ => false 
                };
            }
            else if(clientApplication.FrameworkType == ClientApplicationFrameworkType.React)
            {
                isValid = clientApplication.StyleType switch
                {
                    _ => false
                };
            }
            else
            {
                isValid = false;
            }

            if (!isValid)
            {
                throw new AbpValidationException([new ValidationResult("SitesManagement::Applications:StyleType:IsNotSupportedByFramework", [nameof(ApplicationEntity.StyleType)])]);
            }
        }

        private async Task PublishApplicationUpdated()
        {
            await eventBus.PublishAsync(new ApplicationUpdatedMsg
            {

            });
        }
    }
}


