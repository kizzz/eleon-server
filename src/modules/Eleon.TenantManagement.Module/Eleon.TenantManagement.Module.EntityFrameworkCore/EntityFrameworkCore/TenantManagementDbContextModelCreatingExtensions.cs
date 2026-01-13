using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

public static class TenantManagementDbContextModelCreatingExtensions
{
  public static void ConfigureTenantManagement(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    foreach (var entityType in builder.Model.GetEntityTypes())
    {
      var tableName = entityType.GetTableName();
      if (!IsAbpTable(tableName))
      {
        builder
            .Entity(entityType.Name, x =>
            {
              x.ToTable($"{TenantManagementDbProperties.DbTablePrefix}_{tableName}");
              x.ConfigureByConvention();
            });
      }
    }
  }

  private static bool IsAbpTable(string tableName)
      => tableName.StartsWith("Abp") || tableName == "ExtraPropertyDictionary";
}
