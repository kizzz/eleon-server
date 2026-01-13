using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

[ConnectionStringName(TenantManagementDbProperties.ConnectionStringName)]
public interface ITenantManagementDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
