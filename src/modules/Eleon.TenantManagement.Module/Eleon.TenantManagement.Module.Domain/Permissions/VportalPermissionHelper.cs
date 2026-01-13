using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace VPortal.TenantManagement.Module.Permissions
{
  public class VportalPermissionHelper : ITransientDependency
  {
    private readonly ICurrentUser currentUser;
    private readonly ICurrentTenant currentTenant;
    private readonly IdentityUserManager userManager;

    public VportalPermissionHelper(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager)
    {
      this.currentUser = currentUser;
      this.currentTenant = currentTenant;
      this.userManager = userManager;
    }

    public async Task EnsureHostAdmin()
    {
      bool isHost = currentTenant.Id == null;
      if (!isHost)
      {
        throw new AbpAuthorizationException("This can only be done by the host tenant");
      }

      await EnsureAdmin();
    }

    public async Task EnsureAdmin()
    {
      if (!await IsAdmin(currentUser.Id.Value))
      {
        throw new AbpAuthorizationException("This can only be done by admin");
      }
    }

    public async Task<bool> IsAdmin(Guid userId)
    {
      var identityUser = await userManager.FindByIdAsync(userId.ToString());
      return await userManager.IsInRoleAsync(identityUser, MigrationConsts.AdminRoleNameDefaultValue);
    }
  }
}
