using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Auditor.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IAuditorDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
