using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using VPortal.Otp.Module.Entities;

namespace VPortal.Otp.Module.EntityFrameworkCore;

[ConnectionStringName(OtpDbProperties.ConnectionStringName)]
public class OtpDbContext : AbpDbContext<OtpDbContext>, IOtpDbContext
{
  public DbSet<OtpEntity> Otps { get; set; }

  public OtpDbContext(DbContextOptions<OtpDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureOtp();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
