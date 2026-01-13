using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.FileManager.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedIsActiveFieldToArchive : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
          name: "IsActive",
          table: "EcFileArchives",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "IsActive",
          table: "EcFileArchives");
    }
  }
}
