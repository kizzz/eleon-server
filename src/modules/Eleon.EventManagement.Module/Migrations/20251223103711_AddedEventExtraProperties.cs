using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.EventManagement.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddedEventExtraProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "EcEventManagementEvents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "EcEventManagementEvents");
        }
    }
}
