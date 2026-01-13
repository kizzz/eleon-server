using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Infrastructure.Module.Migrations
{
  /// <inheritdoc />
  public partial class InitInfrastructure : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcAddresses",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            EntityUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ParentUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AddressName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CardCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            City = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
            State = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ObjType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Building = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AddresType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AddrType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StreetNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AddressHashCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcAddresses", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcCountries",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcCountries", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcDashboardSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            XCoordinate = table.Column<int>(type: "int", nullable: true),
            YCoordinate = table.Column<int>(type: "int", nullable: true),
            Cols = table.Column<int>(type: "int", nullable: true),
            MaxItemCols = table.Column<int>(type: "int", nullable: true),
            MinItemCols = table.Column<int>(type: "int", nullable: true),
            Rows = table.Column<int>(type: "int", nullable: true),
            MaxItemRows = table.Column<int>(type: "int", nullable: true),
            MinItemRows = table.Column<int>(type: "int", nullable: true),
            Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Template = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            DragEnabled = table.Column<bool>(type: "bit", nullable: false),
            ResizeEnabled = table.Column<bool>(type: "bit", nullable: false),
            CompactEnabled = table.Column<bool>(type: "bit", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcDashboardSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFeatureSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
            IsRequired = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFeatureSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcSeriaNumbers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Prefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LastUsedNumber = table.Column<long>(type: "bigint", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcSeriaNumbers", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcAddresses");

      migrationBuilder.DropTable(
          name: "EcCountries");

      migrationBuilder.DropTable(
          name: "EcDashboardSettings");

      migrationBuilder.DropTable(
          name: "EcFeatureSettings");

      migrationBuilder.DropTable(
          name: "EcSeriaNumbers");
    }
  }
}
