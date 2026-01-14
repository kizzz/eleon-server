using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.JobScheduler.Module.Migrations
{
    /// <inheritdoc />
    public partial class MovedNextRunToTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextRunTimeUtc",
                table: "EcTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunUtc",
                table: "EcTriggers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextRunUtc",
                table: "EcTriggers");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunTimeUtc",
                table: "EcTasks",
                type: "datetime2",
                nullable: true);
        }
    }
}
