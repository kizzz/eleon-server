using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IJobSchedulerDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
