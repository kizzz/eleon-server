using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Accounting.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101413 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcAccountingModules",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DocEntry = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAccountingModules", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcAccounts",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DocEntry = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DataSourceUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DataSourceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CompanyUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            OrganizationUnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            AccountCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AccountNameEng = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Type = table.Column<int>(type: "int", nullable: false),
            AccountStatus = table.Column<int>(type: "int", nullable: false),
            DocumentStatus = table.Column<int>(type: "int", nullable: false),
            BillingInformationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ResellerRefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            AccountTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ResellerType = table.Column<int>(type: "int", nullable: false),
            AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AdminPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationFromTenant = table.Column<bool>(type: "bit", nullable: false),
            AdminUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAccounts", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcBillingInformations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CompanyCID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            City = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StateOrProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ContactPersonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ContactPersonEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ContactPersonTelephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PaymentMethod = table.Column<int>(type: "int", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBillingInformations", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcPackageTemplates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            PackageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DocEntry = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsShowToReseller = table.Column<bool>(type: "bit", nullable: false),
            BillingPeriodType = table.Column<int>(type: "int", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPackageTemplates", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcModuleUserTypeConfigurationEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            AccountingModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserType = table.Column<int>(type: "int", nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            LimitationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcModuleUserTypeConfigurationEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcModuleUserTypeConfigurationEntity_EcAccountingModules_AccountingModuleEntityId",
                      column: x => x.AccountingModuleEntityId,
                      principalTable: "EcAccountingModules",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcAccountPackageEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            NextBillingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastBillingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            AutoSuspention = table.Column<bool>(type: "bit", nullable: false),
            AutoRenewal = table.Column<bool>(type: "bit", nullable: false),
            ExpiringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            PackageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PackageTemplateEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OneTimeDiscount = table.Column<double>(type: "float", nullable: false),
            PermanentDiscount = table.Column<double>(type: "float", nullable: false),
            BillingPeriodType = table.Column<int>(type: "int", nullable: false),
            AccountEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAccountPackageEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcAccountPackageEntity_EcAccounts_AccountEntityId",
                      column: x => x.AccountEntityId,
                      principalTable: "EcAccounts",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcInvoiceEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            AccountEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CompanyCID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingState = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
            BillingPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcInvoiceEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcInvoiceEntity_EcAccounts_AccountEntityId",
                      column: x => x.AccountEntityId,
                      principalTable: "EcAccounts",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcPackageTemplateModuleEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            PackageTemplateEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsEnabledByDefault = table.Column<bool>(type: "bit", nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPackageTemplateModuleEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcPackageTemplateModuleEntity_EcPackageTemplates_PackageTemplateEntityId",
                      column: x => x.PackageTemplateEntityId,
                      principalTable: "EcPackageTemplates",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcAccountPackageModuleEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            AccountPackageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            LimitationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAccountPackageModuleEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcAccountPackageModuleEntity_EcAccountPackageEntity_AccountPackageEntityId",
                      column: x => x.AccountPackageEntityId,
                      principalTable: "EcAccountPackageEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcInvoiceRowEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            InvoiceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Count = table.Column<int>(type: "int", nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcInvoiceRowEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcInvoiceRowEntity_EcInvoiceEntity_InvoiceEntityId",
                      column: x => x.InvoiceEntityId,
                      principalTable: "EcInvoiceEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcReceiptEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            InvoiceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            PaymentType = table.Column<int>(type: "int", nullable: false),
            Transaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcReceiptEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcReceiptEntity_EcInvoiceEntity_InvoiceEntityId",
                      column: x => x.InvoiceEntityId,
                      principalTable: "EcInvoiceEntity",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcAccountPackageEntity_AccountEntityId",
          table: "EcAccountPackageEntity",
          column: "AccountEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcAccountPackageModuleEntity_AccountPackageEntityId",
          table: "EcAccountPackageModuleEntity",
          column: "AccountPackageEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcInvoiceEntity_AccountEntityId",
          table: "EcInvoiceEntity",
          column: "AccountEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcInvoiceRowEntity_InvoiceEntityId",
          table: "EcInvoiceRowEntity",
          column: "InvoiceEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcModuleUserTypeConfigurationEntity_AccountingModuleEntityId",
          table: "EcModuleUserTypeConfigurationEntity",
          column: "AccountingModuleEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcPackageTemplateModuleEntity_PackageTemplateEntityId",
          table: "EcPackageTemplateModuleEntity",
          column: "PackageTemplateEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcReceiptEntity_InvoiceEntityId",
          table: "EcReceiptEntity",
          column: "InvoiceEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcAccountPackageModuleEntity");

      migrationBuilder.DropTable(
          name: "EcBillingInformations");

      migrationBuilder.DropTable(
          name: "EcInvoiceRowEntity");

      migrationBuilder.DropTable(
          name: "EcModuleUserTypeConfigurationEntity");

      migrationBuilder.DropTable(
          name: "EcPackageTemplateModuleEntity");

      migrationBuilder.DropTable(
          name: "EcReceiptEntity");

      migrationBuilder.DropTable(
          name: "EcAccountPackageEntity");

      migrationBuilder.DropTable(
          name: "EcAccountingModules");

      migrationBuilder.DropTable(
          name: "EcPackageTemplates");

      migrationBuilder.DropTable(
          name: "EcInvoiceEntity");

      migrationBuilder.DropTable(
          name: "EcAccounts");
    }
  }
}
