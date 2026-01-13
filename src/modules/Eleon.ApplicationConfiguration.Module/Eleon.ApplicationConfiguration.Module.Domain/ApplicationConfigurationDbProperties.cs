using Migrations.Module;

namespace VPortal.ApplicationConfiguration.Module;

public static class ApplicationConfigurationDbProperties
{
  public static string DbTablePrefix { get; set; } = "AppConfig";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
