using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.LanguageManagement.Module.EntityFrameworkCore;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public interface ILanguageManagementDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
