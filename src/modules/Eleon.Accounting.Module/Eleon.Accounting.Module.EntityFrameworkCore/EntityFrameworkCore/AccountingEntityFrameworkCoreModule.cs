using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Repositories;

namespace VPortal.Accounting.Module.EntityFrameworkCore;

[DependsOn(
    typeof(AccountingDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class AccountingEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<AccountingDbContext>(options =>
    {
      options.AddRepository<AccountEntity, AccountRepository>();
      options.AddRepository<BillingInformationEntity, BillingInformationRepository>();
      options.AddRepository<PackageTemplateEntity, PackageTemplateRepository>();
    });
  }
}
