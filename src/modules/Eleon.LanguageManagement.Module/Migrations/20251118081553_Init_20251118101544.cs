using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.LanguageManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101544 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcLanguages",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CultureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UiCultureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TwoLetterISOLanguageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcLanguages", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcLocalizationEntries",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CultureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ResourceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcLocalizationEntries", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcLanguages");

      migrationBuilder.DropTable(
          name: "EcLocalizationEntries");
    }
  }
}
