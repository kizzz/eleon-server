using Migrations.Module;

namespace VPortal.Storage.Module;

public static class ProviderModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "Module";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
