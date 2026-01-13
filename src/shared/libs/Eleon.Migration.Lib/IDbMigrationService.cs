namespace Migrations.Module
{
  public interface IDbMigrationService
  {
    Task MigrateTenantAsync(Guid id, string adminEmail = MigrationConsts.AdminEmailDefaultValue, string adminPassword = MigrationConsts.AdminPasswordDefaultValue, string adminUserName = MigrationConsts.AdminUserNameDefaultValue);
    Task MigrateAsync();
  }
}
