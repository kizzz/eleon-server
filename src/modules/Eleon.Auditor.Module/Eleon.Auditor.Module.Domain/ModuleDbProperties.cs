using Migrations.Module;

namespace VPortal.Auditor.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "Module";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
