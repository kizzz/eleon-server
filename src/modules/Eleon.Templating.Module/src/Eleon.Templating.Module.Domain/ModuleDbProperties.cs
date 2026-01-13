using Migrations.Module;

namespace Eleon.Templating.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "Templating";

  public static string? DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
