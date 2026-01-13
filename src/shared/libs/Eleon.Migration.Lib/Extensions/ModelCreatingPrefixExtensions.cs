using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Common.Module.Extensions
{
  public static class ModelCreatingPrefixExtensions
  {
    public static void ConfigurePrefix(
        this ModelBuilder builder,
        string prefix)
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
                x.ToTable($"{prefix}_{tableName}");
                x.ConfigureByConvention();
              });
        }
      }
    }

    private static bool IsAbpTable(string tableName)
        => tableName.StartsWith("Abp") || tableName == "ExtraPropertyDictionary";
  }
}
