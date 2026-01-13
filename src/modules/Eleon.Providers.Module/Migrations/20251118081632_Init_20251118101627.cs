using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Storage.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101627 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcStorageProviders",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Type = table.Column<int>(type: "int", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            IsTested = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcStorageProviders", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcStorageProviderSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            StorageProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StorageProviderEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcStorageProviderSettings", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcStorageProviderSettings_EcStorageProviders_StorageProviderEntityId",
                      column: x => x.StorageProviderEntityId,
                      principalTable: "EcStorageProviders",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcStorageProviderSettings_StorageProviderEntityId",
          table: "EcStorageProviderSettings",
          column: "StorageProviderEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcStorageProviderSettings");

      migrationBuilder.DropTable(
          name: "EcStorageProviders");
    }
  }
}
