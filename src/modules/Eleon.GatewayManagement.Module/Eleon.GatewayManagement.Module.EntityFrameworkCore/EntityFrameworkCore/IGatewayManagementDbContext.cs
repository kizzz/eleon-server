using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.GatewayManagement.Module.EntityFrameworkCore;

[ConnectionStringName(GatewayManagementDbProperties.ConnectionStringName)]
public interface IGatewayManagementDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
