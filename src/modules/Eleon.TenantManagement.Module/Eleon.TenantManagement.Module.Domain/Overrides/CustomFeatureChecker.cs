//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp.EntityFrameworkCore;
//using Volo.Abp.Features;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.TenantManagement;
//using Volo.Abp.TenantManagement.EntityFrameworkCore;

//namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
//{
//    public class CustomFeatureChecker : FeatureChecker, IFeatureChecker
//    {
//        private readonly ICurrentTenant currentTenant;
//        private readonly IDbContextProvider<ITenantManagementDbContext> tenantManagementDbContextProvider;
//        private readonly IConfiguration configuration;

//        public CustomFeatureChecker(IOptions<AbpFeatureOptions> options, IServiceProvider serviceProvider, IFeatureDefinitionManager featureDefinitionManager,
//            ICurrentTenant currentTenant,IDbContextProvider<ITenantManagementDbContext> tenantManagementDbContextProvider, IFeatureValueProviderManager featureValueProviderManager,
//            IConfiguration configuration) : base(options, serviceProvider, featureDefinitionManager, featureValueProviderManager)
//        {
//            this.currentTenant = currentTenant;
//            this.tenantManagementDbContextProvider = tenantManagementDbContextProvider;
//            this.configuration = configuration;
//        }

//        public override async Task<string> GetOrNullAsync(string name)
//        {
//            if (!IsDefault(currentTenant.Id))
//            {
//                var tenantDetails = await GetTenantDetails(currentTenant.Id);
//                var parentIdUnparsed = (string)tenantDetails.ExtraProperties.GetValueOrDefault("ParentId");
//                Guid? parentId = parentIdUnparsed.IsNullOrEmpty() ? null : Guid.Parse(parentIdUnparsed);
//                if (tenantDetails != null && parentId.HasValue)
//                {
//                    using (currentTenant.Change(parentId))
//                    {
//                        return await GetOrNullAsyncWithTenant(name);
//                    }
//                }
//            }

//            return await GetOrNullAsyncWithTenant(name);
//        }

//        private async Task<string> GetOrNullAsyncWithTenant(string name)
//        {
//            var featureDefinition = await FeatureDefinitionManager.GetAsync(name);
//            var providers = featureDefinition.AllowedProviders;

//            if (featureDefinition.AllowedProviders.Any())
//            {
//                providers = providers.Where(p => featureDefinition.AllowedProviders.Contains(p.));
//            }

//            return await GetOrNullValueFromProvidersAsync(providers, featureDefinition);
//        }

//        private async Task<Tenant> GetTenantDetails(Guid? id)
//        {
//            using (currentTenant.ChangeDefault())
//            {
//                var tenantManagementDbContext = await tenantManagementDbContextProvider.GetDbContextAsync();
//                return tenantManagementDbContext
//                    .Tenants
//                    .IncludeDetails(false)
//                    .OrderBy(t => t.Id)
//                    .FirstOrDefault(t => t.Id == id);
//            }
//        }

//        public bool IsDefault(Guid? id)
//        {
//            return id == null || configuration.GetValue<Guid>("RootTenantId") == id || id == Guid.Empty;
//        }
//    }
//}
