using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
// using Volo.Abp.IdentityServer.ApiResources;
// using Volo.Abp.IdentityServer.EntityFrameworkCore;
//using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
//using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
//using Volo.Saas.EntityFrameworkCore;

namespace VPortal.EntityFrameworkCore;

public abstract class VPortalDbContextBase<TDbContext> : AbpDbContext<TDbContext>
    where TDbContext : DbContext
{
  public VPortalDbContextBase(DbContextOptions<TDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    /* Include modules to your migration db context */

    builder.ConfigureSettingManagement();
    builder.ConfigureBackgroundJobs();
    builder.ConfigureAuditLogging();
    //builder.ConfigureIdentity();
    // [ABP-PRO]:
    //builder.ConfigureIdentityPro();
    // builder.ConfigureIdentityServer();
    builder.ConfigureFeatureManagement();
    // [ABP-PRO]:
    //builder.ConfigureLanguageManagement();
    //builder.ConfigureSaas();
    builder.ConfigureTenantManagement();
    //builder.ConfigureTextTemplateManagement();
    builder.ConfigureBlobStoring();
    // CMS Kit

    /* Configure your own tables/entities inside here */

    //builder.Entity<YourEntity>(b =>
    //{
    //    b.ToTable(MigrationConsts.DbTablePrefix + "YourEntities", MigrationConsts.DbSchema);
    //    b.ConfigureByConvention(); //auto configure for the base class props
    //    //...
    //});

    //if (builder.IsHostDatabase())
    //{
    //    /* Tip: Configure mappings like that for the entities only available in the host side,
    //     * but should not be in the tenant databases. */
    //}
  }
}
