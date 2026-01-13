using Migrations.Module;

namespace VPortal.Collaboration.Feature.Module;

public static class CollaborationDbProperties
{
  public static string DbTablePrefix { get; set; } = "Chat";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
