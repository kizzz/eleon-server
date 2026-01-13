using Migrations.Module;

namespace VPortal.Otp;

public static class OtpDbProperties
{
  public static string DbTablePrefix { get; set; } = "Otp";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
