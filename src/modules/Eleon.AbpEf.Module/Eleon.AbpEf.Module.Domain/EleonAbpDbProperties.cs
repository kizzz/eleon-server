using Migrations.Module;

namespace VPortal.TenantManagement.Module;

public static class EleonAbpDbProperties
{
  public static string DbTablePrefix { get; set; } = "EleoncoreMultiTenancy";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
