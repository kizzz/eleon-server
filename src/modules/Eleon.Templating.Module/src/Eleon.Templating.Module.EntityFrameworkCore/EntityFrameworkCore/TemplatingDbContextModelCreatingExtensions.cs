using Eleon.Templating.Module.Templates;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eleon.Templating.Module.EntityFrameworkCore;

public static class TemplatingDbContextModelCreatingExtensions
{
  public static void ConfigureModule(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    builder.Entity<Template>(b =>
    {
      //Configure table & schema name
      b.ToTable(ModuleDbProperties.DbTablePrefix + "Templates", ModuleDbProperties.DbSchema);

      b.ConfigureByConvention();

      //Properties
      b.Property(t => t.Name).IsRequired().HasMaxLength(TemplateConstants.MaxNameLength);
      b.Property(t => t.TemplateContent).IsRequired().HasMaxLength(TemplateConstants.MaxTemplateLength);
      b.Property(t => t.RequiredPlaceholders).HasMaxLength(TemplateConstants.MaxRequiredPlaceholdersLength);
      b.Property(t => t.Type).IsRequired();
      b.Property(t => t.Format).IsRequired();
      b.Property(t => t.IsSystem).IsRequired();

      //Indexes
      b.HasIndex(t => new { t.Name, t.Type }).IsUnique();
      b.HasIndex(t => t.Type);
      b.HasIndex(t => t.IsSystem);
      b.HasIndex(t => t.CreationTime);
    });
  }
}
