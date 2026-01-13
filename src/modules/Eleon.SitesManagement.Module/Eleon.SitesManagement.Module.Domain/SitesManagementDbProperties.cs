using Migrations.Module;

namespace VPortal.SitesManagement.Module;

public static class SitesManagementDbProperties
{
  public static string DbTablePrefix { get; set; } = "SitesManagement";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}


