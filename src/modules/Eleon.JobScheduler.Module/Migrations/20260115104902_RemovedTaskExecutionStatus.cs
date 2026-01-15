using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.JobScheduler.Module.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTaskExecutionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EcTaskExecutions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EcTaskExecutions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
