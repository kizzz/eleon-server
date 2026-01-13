using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.FileManager.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedIsPersonalizedArchive : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
          name: "IsPersonalizedArchive",
          table: "EcFileArchives",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "IsPersonalizedArchive",
          table: "EcFileArchives");
    }
  }
}
