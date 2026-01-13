using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Accounting.Module.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredAccountingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcAccountPackageModuleEntity");

            migrationBuilder.DropTable(
                name: "EcModuleUserTypeConfigurationEntity");

            migrationBuilder.DropTable(
                name: "EcAccountingModules");

            migrationBuilder.DropIndex(
                name: "IX_EcReceiptEntity_InvoiceEntityId",
                table: "EcReceiptEntity");

            migrationBuilder.DropColumn(
                name: "IsShowToReseller",
                table: "EcPackageTemplates");

            migrationBuilder.DropColumn(
                name: "IsEnabledByDefault",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "ModuleEntityId",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "AccountCurrency",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "AccountNameEng",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "AccountTenantId",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "AdminEmail",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "AdminPassword",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "AdminUserName",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "CreationFromTenant",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "DocEntry",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "DocumentStatus",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "ResellerRefId",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "ResellerType",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "EcAccounts");

            migrationBuilder.DropColumn(
                name: "PackageName",
                table: "EcAccountPackageEntity");

            migrationBuilder.RenameColumn(
                name: "DocEntry",
                table: "EcPackageTemplates",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "MaxMembers",
                table: "EcPackageTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackageType",
                table: "EcPackageTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "EcPackageTemplates",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EcPackageTemplateModuleEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModuleData",
                table: "EcPackageTemplateModuleEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModuleType",
                table: "EcPackageTemplateModuleEntity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RefId",
                table: "EcPackageTemplateModuleEntity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "EcAccounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "PermanentDiscount",
                table: "EcAccountPackageEntity",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "OneTimeDiscount",
                table: "EcAccountPackageEntity",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "EcMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AccountEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountPackageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcMember_EcAccountPackageEntity_AccountPackageEntityId",
                        column: x => x.AccountPackageEntityId,
                        principalTable: "EcAccountPackageEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EcMember_EcAccounts_AccountEntityId",
                        column: x => x.AccountEntityId,
                        principalTable: "EcAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcReceiptEntity_InvoiceEntityId",
                table: "EcReceiptEntity",
                column: "InvoiceEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EcAccounts_BillingInformationId",
                table: "EcAccounts",
                column: "BillingInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_EcAccountPackageEntity_PackageTemplateEntityId",
                table: "EcAccountPackageEntity",
                column: "PackageTemplateEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EcMember_AccountEntityId",
                table: "EcMember",
                column: "AccountEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EcMember_AccountPackageEntityId",
                table: "EcMember",
                column: "AccountPackageEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_EcAccountPackageEntity_EcPackageTemplates_PackageTemplateEntityId",
                table: "EcAccountPackageEntity",
                column: "PackageTemplateEntityId",
                principalTable: "EcPackageTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EcAccounts_EcBillingInformations_BillingInformationId",
                table: "EcAccounts",
                column: "BillingInformationId",
                principalTable: "EcBillingInformations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EcAccountPackageEntity_EcPackageTemplates_PackageTemplateEntityId",
                table: "EcAccountPackageEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_EcAccounts_EcBillingInformations_BillingInformationId",
                table: "EcAccounts");

            migrationBuilder.DropTable(
                name: "EcMember");

            migrationBuilder.DropIndex(
                name: "IX_EcReceiptEntity_InvoiceEntityId",
                table: "EcReceiptEntity");

            migrationBuilder.DropIndex(
                name: "IX_EcAccounts_BillingInformationId",
                table: "EcAccounts");

            migrationBuilder.DropIndex(
                name: "IX_EcAccountPackageEntity_PackageTemplateEntityId",
                table: "EcAccountPackageEntity");

            migrationBuilder.DropColumn(
                name: "MaxMembers",
                table: "EcPackageTemplates");

            migrationBuilder.DropColumn(
                name: "PackageType",
                table: "EcPackageTemplates");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EcPackageTemplates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "ModuleData",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "ModuleType",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "EcPackageTemplateModuleEntity");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "EcAccounts");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "EcPackageTemplates",
                newName: "DocEntry");

            migrationBuilder.AddColumn<bool>(
                name: "IsShowToReseller",
                table: "EcPackageTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabledByDefault",
                table: "EcPackageTemplateModuleEntity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleEntityId",
                table: "EcPackageTemplateModuleEntity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AccountCurrency",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNameEng",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountTenantId",
                table: "EcAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminPassword",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminUserName",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CreationFromTenant",
                table: "EcAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DocEntry",
                table: "EcAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentStatus",
                table: "EcAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ResellerRefId",
                table: "EcAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResellerType",
                table: "EcAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "EcAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "PermanentDiscount",
                table: "EcAccountPackageEntity",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "OneTimeDiscount",
                table: "EcAccountPackageEntity",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "PackageName",
                table: "EcAccountPackageEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EcAccountingModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocEntry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcAccountingModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcAccountPackageModuleEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountPackageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LimitationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                name: "EcModuleUserTypeConfigurationEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountingModuleEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LimitationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_EcReceiptEntity_InvoiceEntityId",
                table: "EcReceiptEntity",
                column: "InvoiceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EcAccountPackageModuleEntity_AccountPackageEntityId",
                table: "EcAccountPackageModuleEntity",
                column: "AccountPackageEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EcModuleUserTypeConfigurationEntity_AccountingModuleEntityId",
                table: "EcModuleUserTypeConfigurationEntity",
                column: "AccountingModuleEntityId");
        }
    }
}
