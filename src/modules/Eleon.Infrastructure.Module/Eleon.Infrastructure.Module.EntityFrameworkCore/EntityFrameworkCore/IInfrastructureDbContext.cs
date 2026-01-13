using Core.Infrastructure.Module.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IInfrastructureDbContext : IEfCoreDbContext
{
  //DbSet<UnitPermissionEntity> UnitPermissions { get; }
  //DbSet<CompanyEntity> Companies { get; }
  DbSet<SeriaNumberEntity> SeriaNumbers { get; set; }
  DbSet<AddressEntity> Addresses { get; set; }
  DbSet<CountryEntity> Countries { get; set; }

  DbSet<FeatureSettingEntity> FeatureSettings { get; }
  DbSet<DashboardSettingEntity> DashboardSettings { get; set; }
}
