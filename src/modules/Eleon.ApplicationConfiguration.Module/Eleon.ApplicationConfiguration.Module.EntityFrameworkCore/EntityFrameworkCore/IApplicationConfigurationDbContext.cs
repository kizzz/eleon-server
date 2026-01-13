using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

[ConnectionStringName(ApplicationConfigurationDbProperties.ConnectionStringName)]
public interface IApplicationConfigurationDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
