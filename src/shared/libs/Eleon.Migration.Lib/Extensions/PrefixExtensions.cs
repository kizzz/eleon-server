using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;
using Volo.Abp.EntityFrameworkCore.Modeling;
namespace SharedCollector.modules.Migration.Module.Extensions

{
  public static class ModelBuilderPrefixExtensions
  {
    public static void ConfigureEntitiesWithPrefix(this ModelBuilder modelBuilder, DbContext context, string prefix)
    {
      // 1. Explicitly configure DbSet<T> entities
      var dbSetProperties = context.GetType()
          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => p.PropertyType.IsGenericType &&
                      p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

      var configuredEntities = new HashSet<Type>();

      foreach (var dbSetProperty in dbSetProperties)
      {
        var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];

        if (entityType.Namespace != null && entityType.Namespace.StartsWith("Volo.Abp") && !dbSetProperty.Name.StartsWith(prefix))
        {
          modelBuilder.Entity(entityType, builder =>
          {
            builder.ConfigureByConvention();
            builder.ToTable(prefix + (dbSetProperty.Name.StartsWith("Abp") ? dbSetProperty.Name.Substring(3) : dbSetProperty.Name));
          });
          //continue;
        }
        else
        {
          modelBuilder.Entity(entityType, builder =>
          {
            builder.ConfigureByConvention();
            builder.ToTable(prefix + dbSetProperty.Name);
          });
        }

        configuredEntities.Add(entityType);
      }

      // 2. Fallback for auto-discovered entities (e.g., nested/navigation ones)
      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
        var clrType = entity.ClrType;
        if (configuredEntities.Contains(clrType))
          continue;

        var currentTableName = entity.GetTableName() ?? string.Empty;

        if (clrType.Namespace != null && clrType.Namespace.StartsWith("Volo.Abp") && !currentTableName.StartsWith(prefix))
        {
          entity.SetTableName(prefix + (currentTableName.StartsWith("Abp") ? currentTableName.Substring(3) : currentTableName));
          //continue;
        }
        else if (!string.IsNullOrEmpty(currentTableName) && !currentTableName.StartsWith(prefix))
        {
          entity.SetTableName(prefix + currentTableName);
        }
      }
    }
  }
}

