using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace VPortal.LanguageManagement.Module.EntityFrameworkCore;

public static class LanguageManagementDbContextModelCreatingExtensions
{
  public static void ConfigureLanguageManagement(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    /* Configure all entities here. Example:

    builder.Entity<Question>(b =>
    {
        //Configure table & schema name
        b.ToTable(LanguageManagementDbProperties.DbTablePrefix + "Questions", LanguageManagementDbProperties.DbSchema);

        b.ConfigureByConvention();

        //Properties
        b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

        //Relations
        b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

        //Indexes
        b.HasIndex(q => q.CreationTime);
    });
    */
  }
}
