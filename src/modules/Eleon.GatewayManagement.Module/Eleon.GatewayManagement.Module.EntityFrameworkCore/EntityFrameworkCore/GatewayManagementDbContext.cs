using Microsoft.EntityFrameworkCore;
using GatewayManagement.Module.Entities;
using Volo.Abp.Data;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.EntityFrameworkCore;
using EventBusManagement.Module.EntityFrameworkCore;

namespace VPortal.GatewayManagement.Module.EntityFrameworkCore;

[ConnectionStringName(GatewayManagementDbProperties.ConnectionStringName)]
public class GatewayManagementDbContext : AbpDbContext<GatewayManagementDbContext>, IGatewayManagementDbContext
{
  public DbSet<GatewayEntity> Gateways { get; set; }
  public DbSet<GatewayRegistrationKeyEntity> GatewayRegistrationKeys { get; set; }
  public DbSet<GatewayPrivateDetailsEntity> GatewayPrivateDetails { get; set; }

  public DbSet<EventBusEntity> EventBuses { get; set; }

  public GatewayManagementDbContext(DbContextOptions<GatewayManagementDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureGatewayManagement();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");

  }
}
