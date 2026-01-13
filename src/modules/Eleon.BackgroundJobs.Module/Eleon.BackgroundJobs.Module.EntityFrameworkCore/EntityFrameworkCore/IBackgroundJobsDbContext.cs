using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public interface IBackgroundJobsDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
  DbSet<BackgroundJobEntity> EleoncoreBackgroundJobs { get; set; }
}
