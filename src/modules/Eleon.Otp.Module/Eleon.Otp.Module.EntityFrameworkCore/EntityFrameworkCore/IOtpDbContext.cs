using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Otp.Module.EntityFrameworkCore;

[ConnectionStringName(OtpDbProperties.ConnectionStringName)]
public interface IOtpDbContext : IEfCoreDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * DbSet<Question> Questions { get; }
   */
}
