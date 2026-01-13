using Migrations.Module;

namespace VPortal.LanguageManagement;

public static class LanguageManagementDbProperties
{
  public static string DbTablePrefix { get; set; } = "LanguageManagement";

  public static string DbSchema { get; set; } = null;

  public const string ConnectionStringName = MigrationConsts.DefaultConnectionStringName;
}
