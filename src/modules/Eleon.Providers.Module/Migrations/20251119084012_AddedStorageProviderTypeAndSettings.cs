using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Storage.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedStorageProviderTypeAndSettings : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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

      migrationBuilder.CreateTable(
          name: "EcStorageProviderTypes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Parent = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStorageProviderTypes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcStorageProviderSettingTypes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StorageProviderTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Type = table.Column<int>(type: "int", nullable: false),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Hidden = table.Column<bool>(type: "bit", nullable: false),
            Required = table.Column<bool>(type: "bit", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStorageProviderSettingTypes", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStorageProviderSettingTypes_EcStorageProviderTypes_StorageProviderTypeId",
                      column: x => x.StorageProviderTypeId,
                      principalTable: "EcStorageProviderTypes",
                      principalColumn: "Id");
          });

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

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviderSettingTypes_StorageProviderTypeId",
          table: "EcStorageProviderSettingTypes",
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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
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

      migrationBuilder.DropTable(
          name: "EcStorageProviderSettingTypes");

      migrationBuilder.DropTable(
          name: "EcStorageProviderTypes");

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
          table: "EcStorageProviderSettings");

      migrationBuilder.DropColumn(
          name: "TypeId",
          table: "EcStorageProviderSettings");

      migrationBuilder.DropColumn(
          name: "StorageProviderTypeId",
          table: "EcStorageProviders");
    }
  }
}
