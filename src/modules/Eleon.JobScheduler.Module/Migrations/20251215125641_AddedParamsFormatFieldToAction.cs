using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.JobScheduler.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedParamsFormatFieldToAction : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "ParamsFormat",
          table: "EcActions",
          type: "int",
          nullable: false,
          defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "ParamsFormat",
          table: "EcActions");
    }
  }
}
