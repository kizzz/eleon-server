using Migrations.Module;

namespace VPortal.Google;

public static class GoogleDbProperties
{
  public static string DbTablePrefix { get; set; } = "Google";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
