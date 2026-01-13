using Migrations.Module;

namespace VPortal.Infrastructure.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "VPortal";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
