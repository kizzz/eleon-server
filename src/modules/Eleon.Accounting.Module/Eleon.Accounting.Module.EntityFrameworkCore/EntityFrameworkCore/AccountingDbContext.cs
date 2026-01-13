using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Accounting.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.Accounting.Module.EntityFrameworkCore;

[ConnectionStringName(AccountingDbProperties.ConnectionStringName)]
public class AccountingDbContext : AbpDbContext<AccountingDbContext>, IAccountingDbContext
{

  public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
      : base(options)
  {
  }

  public DbSet<AccountEntity> Accounts { get; set; }
  public DbSet<BillingInformationEntity> BillingInformations { get; set; }
  public DbSet<PackageTemplateEntity> PackageTemplates { get; set; }
  public DbSet<MemberEntity> Members { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureModule();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
