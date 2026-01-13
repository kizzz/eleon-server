using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Storage.Module.Migrations
{
  /// <inheritdoc />
  public partial class RemovedTypeBindings : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_EcStorageProviders_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviders");

      migrationBuilder.DropForeignKey(
          name: "FK_EcStorageProviderSettings_EcStorageProviderSettingTypes_TypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropForeignKey(
          name: "FK_EcStorageProviderSettings_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropForeignKey(
          name: "FK_EcStorageProviderSettingTypes_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes");

      migrationBuilder.DropIndex(
          name: "IX_EcStorageProviderSettingTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes");

      migrationBuilder.DropIndex(
          name: "IX_EcStorageProviderSettings_StorageProviderTypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropIndex(
          name: "IX_EcStorageProviderSettings_TypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropIndex(
          name: "IX_EcStorageProviders_StorageProviderTypeId",
          table: "EcStorageProviders");

      migrationBuilder.DropColumn(
          name: "StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes");

      migrationBuilder.DropColumn(
          name: "StorageProviderTypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropColumn(
          name: "TypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropColumn(
          name: "StorageProviderTypeId",
          table: "EcStorageProviders");

      migrationBuilder.DropColumn(
          name: "Type",
          table: "EcStorageProviders");

      migrationBuilder.AddColumn<string>(
          name: "StorageProviderTypeName",
          table: "EcStorageProviderSettingTypes",
          type: "nvarchar(max)",
          nullable: true);

      migrationBuilder.AddColumn<string>(
          name: "StorageProviderTypeName",
          table: "EcStorageProviders",
          type: "nvarchar(max)",
          nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "StorageProviderTypeName",
          table: "EcStorageProviderSettingTypes");

      migrationBuilder.DropColumn(
          name: "StorageProviderTypeName",
          table: "EcStorageProviders");

      migrationBuilder.AddColumn<Guid>(
          name: "StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes",
          type: "uniqueidentifier",
          nullable: true);

      migrationBuilder.AddColumn<Guid>(
          name: "StorageProviderTypeId",
          table: "EcStorageProviderSettings",
          type: "uniqueidentifier",
          nullable: true);

      migrationBuilder.AddColumn<Guid>(
          name: "TypeId",
          table: "EcStorageProviderSettings",
          type: "uniqueidentifier",
          nullable: true);

      migrationBuilder.AddColumn<Guid>(
          name: "StorageProviderTypeId",
          table: "EcStorageProviders",
          type: "uniqueidentifier",
          nullable: true);

      migrationBuilder.AddColumn<int>(
          name: "Type",
          table: "EcStorageProviders",
          type: "int",
          nullable: false,
          defaultValue: 0);

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviderSettingTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes",
          column: "StorageProviderTypeId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviderSettings_StorageProviderTypeId",
          table: "EcStorageProviderSettings",
          column: "StorageProviderTypeId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviderSettings_TypeId",
          table: "EcStorageProviderSettings",
          column: "TypeId");

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviders_StorageProviderTypeId",
          table: "EcStorageProviders",
          column: "StorageProviderTypeId");

      migrationBuilder.AddForeignKey(
          name: "FK_EcStorageProviders_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviders",
          column: "StorageProviderTypeId",
          principalTable: "EcStorageProviderTypes",
          principalColumn: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_EcStorageProviderSettings_EcStorageProviderSettingTypes_TypeId",
          table: "EcStorageProviderSettings",
          column: "TypeId",
          principalTable: "EcStorageProviderSettingTypes",
          principalColumn: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_EcStorageProviderSettings_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettings",
          column: "StorageProviderTypeId",
          principalTable: "EcStorageProviderTypes",
          principalColumn: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_EcStorageProviderSettingTypes_EcStorageProviderTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes",
          column: "StorageProviderTypeId",
          principalTable: "EcStorageProviderTypes",
          principalColumn: "Id");
    }
  }
}
