using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Templating.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcTemplates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            Format = table.Column<int>(type: "int", nullable: false),
            TemplateContent = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
            RequiredPlaceholders = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            IsSystem = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcTemplates", x => x.Id);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcTemplates_CreationTime",
          table: "EcTemplates",
          column: "CreationTime");

      migrationBuilder.CreateIndex(
          name: "IX_EcTemplates_IsSystem",
          table: "EcTemplates",
          column: "IsSystem");

      migrationBuilder.CreateIndex(
          name: "IX_EcTemplates_Name_Type",
          table: "EcTemplates",
          columns: new[] { "Name", "Type" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcTemplates_Type",
          table: "EcTemplates",
          column: "Type");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcTemplates");
    }
  }
}
