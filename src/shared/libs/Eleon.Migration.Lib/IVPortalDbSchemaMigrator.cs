namespace Common.Module.Migrations;

public interface IVPortalDbSchemaMigrator
{
  Task MigrateAsync();
}
