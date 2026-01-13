using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Storage.Module.EntityFrameworkCore;

[ConnectionStringName(ProviderModuleDbProperties.ConnectionStringName)]
public interface IStorageDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
