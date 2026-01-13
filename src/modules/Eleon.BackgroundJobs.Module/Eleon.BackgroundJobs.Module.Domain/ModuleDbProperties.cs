using Migrations.Module;

namespace VPortal.BackgroundJobs.Module;

public static class ModuleDbProperties
{
  public static string DbTablePrefix { get; set; } = "BackgroundJobs";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
