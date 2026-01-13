using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

[ConnectionStringName(CollaborationDbProperties.ConnectionStringName)]
public interface ICollaborationDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
