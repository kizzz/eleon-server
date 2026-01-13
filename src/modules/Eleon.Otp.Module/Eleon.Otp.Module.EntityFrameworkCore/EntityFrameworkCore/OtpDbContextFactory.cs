using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.Otp.Module.EntityFrameworkCore;

namespace VPortal.Otp.Module.Module.EntityFrameworkCore;

public class OtpDbContextFactory : DefaultDbContextFactoryBase<OtpDbContext>
{
  protected override OtpDbContext CreateDbContext(
      DbContextOptions<OtpDbContext> dbContextOptions)
  {
    return new OtpDbContext(dbContextOptions);
  }
}
