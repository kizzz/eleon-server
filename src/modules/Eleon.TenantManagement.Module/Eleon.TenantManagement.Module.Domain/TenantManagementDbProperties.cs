using Migrations.Module;

namespace VPortal.TenantManagement.Module;

public static class TenantManagementDbProperties
{
  public static string DbTablePrefix { get; set; } = "TenantManagement";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
