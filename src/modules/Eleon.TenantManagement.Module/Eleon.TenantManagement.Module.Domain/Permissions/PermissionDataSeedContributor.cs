//namespace VPortal.TenantManagement.Module.Permissions
//{
//    [Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
//    public class CustomPermissionDataSeedContributor : PermissionDataSeedContributor
//    {
//        public CustomPermissionDataSeedContributor(
//            IPermissionDefinitionManager permissionDefinitionManager,
//            ICurrentTenant currentTenant,
//            IPermissionDataSeeder permissionDataSeeder)
//            : base(permissionDefinitionManager, permissionDataSeeder, currentTenant)
//        {
//        }

//        public override async Task SeedAsync(DataSeedContext context)
//        {
//            using (CurrentTenant.Change(context.TenantId))
//            {
//                var permissions = await GetPermissionDefinitions();
//                await PermissionDataSeeder.SeedAsync(
//                    RolePermissionValueProvider.ProviderName,
//                    "admin",
//                    permissions,
//                    context?.TenantId);
//            }
//        }

//        private async Task<List<string>> GetPermissionDefinitions()
//        {
//            var permissions = await PermissionDefinitionManager.GetPermissionsAsync();

//            permissions = permissions
//                .Where(x => !TenantManagementSpecialPermissions.IsSpecialPermission(x.Name))
//                .ToList();

//            var multiTenancySide = CurrentTenant.Id == null ? MultiTenancySides.Host : MultiTenancySides.Tenant;
//            permissions = permissions
//                .Where(x => x.MultiTenancySide == multiTenancySide || x.MultiTenancySide == MultiTenancySides.Both)
//                .Where(p => !p.Providers.Any() || p.Providers.Contains(RolePermissionValueProvider.ProviderName))
//                .ToList();

//            return permissions.Select(p => p.Name).Distinct().ToList();
//        }
//    }
//}
