using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Accounting.Module.Migrations
{
    /// <inheritdoc />
    public partial class AddedMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcMember");

            migrationBuilder.CreateTable(
                name: "EcMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AccountEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountPackageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcMembers_EcAccountPackageEntity_AccountPackageEntityId",
                        column: x => x.AccountPackageEntityId,
                        principalTable: "EcAccountPackageEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EcMembers_EcAccounts_AccountEntityId",
                        column: x => x.AccountEntityId,
                        principalTable: "EcAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcMembers_AccountEntityId",
                table: "EcMembers",
                column: "AccountEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EcMembers_AccountPackageEntityId",
                table: "EcMembers",
                column: "AccountPackageEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcMembers");

            migrationBuilder.CreateTable(
                name: "EcMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountPackageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcMember_EcAccountPackageEntity_AccountPackageEntityId",
                        column: x => x.AccountPackageEntityId,
                        principalTable: "EcAccountPackageEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EcMember_EcAccounts_AccountEntityId",
                        column: x => x.AccountEntityId,
                        principalTable: "EcAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcMember_AccountEntityId",
                table: "EcMember",
                column: "AccountEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EcMember_AccountPackageEntityId",
                table: "EcMember",
                column: "AccountPackageEntityId");
        }
    }
}
