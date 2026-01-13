using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.EntityFrameworkCore;

[ConnectionStringName(AccountingDbProperties.ConnectionStringName)]
public interface IAccountingDbContext : IEfCoreDbContext
{
  public DbSet<AccountEntity> Accounts { get; set; }
  public DbSet<BillingInformationEntity> BillingInformations { get; set; }
  public DbSet<PackageTemplateEntity> PackageTemplates { get; set; }
}
