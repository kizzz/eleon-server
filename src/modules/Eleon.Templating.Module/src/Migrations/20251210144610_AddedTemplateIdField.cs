using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Templating.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedTemplateIdField : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
          name: "TemplateId",
          table: "EcTemplates",
          type: "nvarchar(max)",
          nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "TemplateId",
          table: "EcTemplates");
    }
  }
}
