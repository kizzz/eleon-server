using Microsoft.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

[ConnectionStringName(SitesManagementDbProperties.ConnectionStringName)]
public class SitesManagementDbContext : AbpDbContext<SitesManagementDbContext>, ISitesManagementDbContext
{
  public DbSet<ModuleEntity> ClientApplicationModules { get; set; }
  public DbSet<ApplicationEntity> ClientApplications { get; set; }
  public DbSet<ApplicationModuleEntity> ClientApplicationModulesRelations { get; set; }
  public DbSet<ApplicationPropertyEntity> ClientApplicationPropertyEntities { get; set; }
  public DbSet<ApplicationMenuItemEntity> ClientApplicationMenuItems { get; set; }
  public DbSet<ApplicationTenantConnectionStringEntity> ApplicationTenantConnectionStrings { get; set; }
  public SitesManagementDbContext(DbContextOptions<SitesManagementDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}


