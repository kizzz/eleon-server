using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.BackgroundJobs.Module.Migrations
{
  /// <inheritdoc />
  public partial class RenamedMaxDelayInMinutesToTimeoutInMinutes : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "MaxDelayInMinutes",
          table: "EcEleoncoreBackgroundJobs",
          newName: "TimeoutInMinutes");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "TimeoutInMinutes",
          table: "EcEleoncoreBackgroundJobs",
          newName: "MaxDelayInMinutes");
    }
  }
}
