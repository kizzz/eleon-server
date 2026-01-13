using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using VPortal.EventManagementModule.Module.EntityFrameworkCore;

namespace Volo.Abp.TenantManagement.EntityFrameworkCore;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class EleonAbpDbContext : AbpDbContext<EleonAbpDbContext>, IEleonAbpModuleDbContext
{
  public DbSet<Tenant> Tenants { get; set; }

  public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

  public DbSet<IdentityUser> Users { get; set; }

  public DbSet<IdentityRole> Roles { get; set; }

  public DbSet<IdentityClaimType> ClaimTypes { get; set; }

  public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

  public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

  public DbSet<IdentityLinkUser> LinkUsers { get; set; }

  public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

  public DbSet<IdentitySession> Sessions { get; set; }

  public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }

  public DbSet<PermissionDefinitionRecord> Permissions { get; set; }

  public DbSet<PermissionGrant> PermissionGrants { get; set; }

  public DbSet<Setting> Settings { get; set; }

  public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }

  public DbSet<BackgroundJobRecord> BackgroundJobs { get; set; }

  public DbSet<AuditLog> AuditLogs { get; set; }

  public DbSet<AuditLogExcelFile> AuditLogExcelFiles { get; set; }

  public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }

  public DbSet<FeatureDefinitionRecord> Features { get; set; }

  public DbSet<FeatureManagement.FeatureValue> FeatureValues { get; set; }

  public EleonAbpDbContext(DbContextOptions<EleonAbpDbContext> options)
        : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureIdentity();
    builder.ConfigurePermissionManagement();
    builder.ConfigureSettingManagement();
    builder.ConfigureTenantManagement();
    builder.ConfigureBackgroundJobs();
    builder.ConfigureAuditLogging();
    builder.ConfigureFeatureManagement();
    builder.ConfigureBlobStoring();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");

    builder.Entity<PermissionGroupDefinitionRecord>().HasIndex(x => x.Name).IsUnique(false);
    builder.Entity<FeatureGroupDefinitionRecord>().HasIndex(x => x.Name).IsUnique(false);
    builder.Entity<PermissionDefinitionRecord>().HasIndex(x => x.Name).IsUnique(false);
    builder.Entity<FeatureDefinitionRecord>().HasIndex(x => x.Name).IsUnique(false);
  }
}
