using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;

namespace SharedModule.modules.Helpers.Module;

/// <summary>
/// Extension methods for configuring concurrency tokens in Entity Framework Core ModelBuilder.
/// </summary>
public static class ModelBuilderConcurrencyExtensions
{
  /// <summary>
  /// Configures concurrency tokens for all entities implementing IHasConcurrencyStamp.
  /// Call this in OnModelCreating to automatically set up optimistic concurrency for child entities.
  /// </summary>
  /// <param name="builder">The ModelBuilder instance</param>
  public static void ConfigureConcurrencyTokens(this ModelBuilder builder)
  {
    foreach (var entityType in builder.Model.GetEntityTypes())
    {
      if (typeof(IHasConcurrencyStamp).IsAssignableFrom(entityType.ClrType))
      {
        builder.Entity(entityType.ClrType, entity =>
        {
          entity.Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
            .IsConcurrencyToken()
            .HasMaxLength(40)
            .IsRequired();
        });
      }
    }
  }
}



