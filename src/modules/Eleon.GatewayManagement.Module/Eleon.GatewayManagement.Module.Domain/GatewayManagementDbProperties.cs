using Migrations.Module;

namespace VPortal.GatewayManagement;

public static class GatewayManagementDbProperties
{
  public static string DbTablePrefix { get; set; } = "GatewayManagement";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
