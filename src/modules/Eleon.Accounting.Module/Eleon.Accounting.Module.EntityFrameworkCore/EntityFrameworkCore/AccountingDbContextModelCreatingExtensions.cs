using Microsoft.EntityFrameworkCore;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.EntityFrameworkCore;

public static class AccountingDbContextModelCreatingExtensions
{
  public static void ConfigureModule(
      this ModelBuilder builder)
  {

    builder.Entity<PackageTemplateModuleEntity>()
   .Property(x => x.IsDeleted)
   .HasDefaultValue(false);

    // Configure decimal precision for AccountEntity
    builder.Entity<AccountEntity>()
        .Property(x => x.CurrentBalance)
        .HasPrecision(18, 2);

    // Configure decimal precision for AccountPackageEntity
    builder.Entity<AccountPackageEntity>()
        .Property(x => x.OneTimeDiscount)
        .HasPrecision(18, 2);

    builder.Entity<AccountPackageEntity>()
        .Property(x => x.PermanentDiscount)
        .HasPrecision(18, 2);

    // Configure decimal precision for PackageTemplateEntity
    builder.Entity<PackageTemplateEntity>()
        .Property(x => x.Price)
        .HasPrecision(18, 2);

    // Configure decimal precision for InvoiceRowEntity
    builder.Entity<InvoiceRowEntity>()
        .Property(x => x.Price)
        .HasPrecision(18, 2);

    // Configure decimal precision for ReceiptEntity
    builder.Entity<ReceiptEntity>()
        .Property(x => x.Amount)
        .HasPrecision(18, 2);
  }
}
