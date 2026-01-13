//using Logging.Module;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TenantSettings.Module.Cache;
//using Volo.Abp;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Domain.Entities;
//using Volo.Abp.EntityFrameworkCore;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.TenantManagement;
//using Volo.Abp.TenantManagement.EntityFrameworkCore;
//using VPortal.TenantManagement.Module.EntityFrameworkCore;

//namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
//{
//    [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
//    public class CustomCurrentTenant : ICurrentTenant, ITransientDependency
//    {
//        public virtual bool IsAvailable => Id.HasValue;

//        public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

//        public string Name => _currentTenantAccessor.Current?.Name;

//        private readonly ICurrentTenantAccessor _currentTenantAccessor;
//        private readonly IConfiguration configuration;
//        private readonly IServiceProvider serviceProvider;
//        private readonly IVportalLogger<CustomCurrentTenant> logger;

//        public CustomCurrentTenant(ICurrentTenantAccessor currentTenantAccessor, IConfiguration configuration,
//            //,
//            IServiceProvider serviceProvider,
//            IVportalLogger<CustomCurrentTenant> logger
//            //Volo.Abp.TenantManagement.EntityFrameworkCore.TenantManagementDbContext tenantManagementDbContext
//            )
//        {
//            _currentTenantAccessor = currentTenantAccessor;
//            this.configuration = configuration;
//            this.serviceProvider = serviceProvider;
//            this.logger = logger;
//        }

//        public IDisposable Change(Guid? targetTenantId, string name = null)
//        {

//            try
//            {
//                if (targetTenantId == Guid.Empty || configuration.GetValue<Guid>("RootTenantId") == targetTenantId)
//                {
//                    return SetCurrent(null, name);
//                }

//                if (targetTenantId != null && Id != targetTenantId)
//                {
//                    return SetCurrent(targetTenantId, name);
//                }
//                else if (targetTenantId == null)
//                {
//                    var tenant = FindRoot(Id);
//                    return SetCurrent(tenant?.Id);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.Capture(e);
//            }
//            finally
//            {
//            }

//            return SetCurrent(Id, Name);
//        }

//        private Tenant FindRoot(Guid? id)
//        {
//            if (id == null || configuration.GetValue<Guid>("RootTenantId") == id || id == Guid.Empty)
//            {
//                return null;
//            }

//            Tenant self = GetTenantDetails(id);

//            if (self == null)
//            {
//                throw new EntityNotFoundException("Root tenant not found.");
//            }

//            var parentIdUnparsed = (string)self.ExtraProperties.GetValueOrDefault("ParentId")?.ToString();
//            Guid? parentId = parentIdUnparsed.IsNullOrEmpty() ? null : Guid.Parse(parentIdUnparsed);
//            var isRoot = (bool?) self.ExtraProperties.GetValueOrDefault("IsRoot");
//            if (isRoot == true)
//            {
//                return self;
//            }

//            return FindRoot(parentId);

//        }
//        private IDisposable SetCurrent(Guid? tenantId, string name = null)
//        {
//            var parentScope = _currentTenantAccessor.Current;
//            _currentTenantAccessor.Current = new BasicTenantInfo(tenantId, name);

//            return new DisposeAction<ValueTuple<ICurrentTenantAccessor, BasicTenantInfo?>>(static (state) =>
//            {
//                var (currentTenantAccessor, parentScope) = state;
//                currentTenantAccessor.Current = parentScope;
//            }, (_currentTenantAccessor, parentScope));
//        }


//        private Tenant GetTenantDetails(Guid? id)   
//        {
//                var tenantSettingsCache = serviceProvider.GetService<TenantCacheService>();
//                return tenantSettingsCache
//                    .GetTenants()
//                    .FirstOrDefault(t => t.Id == id);
//        }
//    }
//}
