using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

public static class SitesManagementDbContextModelCreatingExtensions
{
  public static void ConfigureSitesManagement(
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
              x.ToTable($"{SitesManagementDbProperties.DbTablePrefix}_{tableName}");
              x.ConfigureByConvention();
            });
      }
    }
  }

  private static bool IsAbpTable(string tableName)
      => tableName.StartsWith("Abp") || tableName == "ExtraPropertyDictionary";
}


