using Microsoft.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using VPortal.Identity.Module.Entities;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

[ConnectionStringName(TenantManagementDbProperties.ConnectionStringName)]
public class TenantManagementDbContext : AbpDbContext<TenantManagementDbContext>, ITenantManagementDbContext
{
  public DbSet<TenantSettingEntity> TenantSettings { get; set; }
  public DbSet<ControlDelegationEntity> ControlDelegations { get; set; }
  public DbSet<UserIsolationSettingsEntity> UserIsolationSettings { get; set; }
  public DbSet<UserOtpSettingsEntity> UserOtpSettings { get; set; }
  public DbSet<UserSettingEntity> UserSettings { get; set; }

  public DbSet<ApiKeyEntity> ApiKeys { get; set; }
  public DbSet<UserSessionStateEntity> UserSessionStates { get; set; }
  public DbSet<CustomCredentialsEntity> CustomCredentials { get; set; }

  //#region Entities from the modules
  ////Identity
  //public DbSet<EleoncoreUser> Users { get; set; }
  //public DbSet<IdentityRole> Roles { get; set; }
  //public DbSet<IdentityClaimType> ClaimTypes { get; set; }
  //public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
  //public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
  //public DbSet<IdentityLinkUser> LinkUsers { get; set; }
  //public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
  //#endregion


  //#region Entities from the permission management module
  //// Permission Management
  //public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }
  //public DbSet<PermissionDefinitionRecord> Permissions { get; set; }
  //public DbSet<PermissionGrant> PermissionGrants { get; set; }
  //#endregion

  public TenantManagementDbContext(DbContextOptions<TenantManagementDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    //builder.ConfigureIdentity();
    //builder.ConfigureEleoncoreIdentity();
    //builder.ConfigureTenantManagement();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");

  }
}
