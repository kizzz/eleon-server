using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Otp.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118101610 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcOtps",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Recipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DurationS = table.Column<int>(type: "int", nullable: false),
            RetryAttempt = table.Column<int>(type: "int", nullable: false),
            IsUsed = table.Column<bool>(type: "bit", nullable: false),
            FailedValidationAttempts = table.Column<int>(type: "int", nullable: false),
            IsIgnored = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcOtps", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcOtps");
    }
  }
}
