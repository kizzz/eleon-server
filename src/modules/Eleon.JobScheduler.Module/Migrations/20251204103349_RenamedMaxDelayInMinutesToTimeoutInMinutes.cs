using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.JobScheduler.Module.Migrations
{
  /// <inheritdoc />
  public partial class RenamedMaxDelayInMinutesToTimeoutInMinutes : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "MaxDelayInMinutes",
          table: "EcActions",
          newName: "TimeoutInMinutes");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "TimeoutInMinutes",
          table: "EcActions",
          newName: "MaxDelayInMinutes");
    }
  }
}
