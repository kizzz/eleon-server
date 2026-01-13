using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Google.Module.EntityFrameworkCore;

[ConnectionStringName(GoogleDbProperties.ConnectionStringName)]
public interface IGoogleDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
