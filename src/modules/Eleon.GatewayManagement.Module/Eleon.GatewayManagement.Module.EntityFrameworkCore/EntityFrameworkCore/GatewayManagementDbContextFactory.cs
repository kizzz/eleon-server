using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace VPortal.GatewayManagement.Module.Module.EntityFrameworkCore;

public class GatewayManagementDbContextFactory : DefaultDbContextFactoryBase<GatewayManagementDbContext>
{
  protected override GatewayManagementDbContext CreateDbContext(
      DbContextOptions<GatewayManagementDbContext> dbContextOptions)
  {
    return new GatewayManagementDbContext(dbContextOptions);
  }
}
