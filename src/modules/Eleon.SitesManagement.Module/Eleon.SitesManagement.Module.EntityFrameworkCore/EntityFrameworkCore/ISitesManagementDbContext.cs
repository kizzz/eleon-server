using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

[ConnectionStringName(SitesManagementDbProperties.ConnectionStringName)]
public interface ISitesManagementDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}


