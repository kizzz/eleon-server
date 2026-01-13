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
