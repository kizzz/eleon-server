using Migrations.Module;

namespace VPortal.FileManager.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "FileManager";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
