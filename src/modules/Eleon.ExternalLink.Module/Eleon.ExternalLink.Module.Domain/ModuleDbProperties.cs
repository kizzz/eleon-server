using Migrations.Module;

namespace VPortal.ExternalLink.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "ExternalLink";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
