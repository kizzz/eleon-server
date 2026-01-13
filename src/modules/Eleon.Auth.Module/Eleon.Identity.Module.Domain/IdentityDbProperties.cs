using Migrations.Module;

namespace VPortal.Identity.Module;

public static class IdentityDbProperties
{
  public static string DbTablePrefix { get; set; } = "Identity";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
