using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.FileManager.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedPhysicalRootFolderId : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
          name: "PhysicalRootFolderId",
          table: "EcFileArchives",
          type: "nvarchar(max)",
          nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "PhysicalRootFolderId",
          table: "EcFileArchives");
    }
  }
}
