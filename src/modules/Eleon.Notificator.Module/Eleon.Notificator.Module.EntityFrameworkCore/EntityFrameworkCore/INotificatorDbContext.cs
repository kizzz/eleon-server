using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Notificator.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface INotificatorDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
