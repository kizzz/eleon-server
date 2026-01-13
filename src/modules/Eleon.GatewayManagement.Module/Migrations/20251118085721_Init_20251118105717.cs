using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.GatewayManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118105717 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcEventBuses",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Provider = table.Column<int>(type: "int", nullable: false),
            ProviderOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcEventBuses", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcGatewayPrivateDetails",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            GatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClientKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MachineKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CertificatePemBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcGatewayPrivateDetails", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcGatewayRegistrationKeys",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            GatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            Invalidated = table.Column<bool>(type: "bit", nullable: false),
            Multiuse = table.Column<bool>(type: "bit", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcGatewayRegistrationKeys", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcGateways",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Protocol = table.Column<int>(type: "int", nullable: false),
            IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            MachineHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Port = table.Column<int>(type: "int", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            AllowApplicationOverride = table.Column<bool>(type: "bit", nullable: false),
            EnableGatewayAdmin = table.Column<bool>(type: "bit", nullable: false),
            EventBusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            HealthStatus = table.Column<int>(type: "int", nullable: false),
            LastHealthCheckTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            SelfHostEventBus = table.Column<bool>(type: "bit", nullable: false),
            VpnAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            VpnAdapterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            VpnAdapterGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            VpnPrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            VpnPublicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            VpnDns = table.Column<string>(type: "nvarchar(max)", nullable: true),
            VpnListenPort = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcGateways", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcEventBuses");

      migrationBuilder.DropTable(
          name: "EcGatewayPrivateDetails");

      migrationBuilder.DropTable(
          name: "EcGatewayRegistrationKeys");

      migrationBuilder.DropTable(
          name: "EcGateways");
    }
  }
}
