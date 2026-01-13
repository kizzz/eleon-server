using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Eleon.Templating.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface ITemplatingDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
