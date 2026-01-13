using Migrations.Module;

namespace VPortal.DocMessageLog.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "DocMessageLog";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
