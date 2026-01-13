using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Abp.Module.Migrations
{
  /// <inheritdoc />
  public partial class AddedEleonAbpTables : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcAuditLogExcelFiles",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAuditLogExcelFiles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcAuditLogs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ImpersonatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ImpersonatorUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ImpersonatorTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ImpersonatorTenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExecutionDuration = table.Column<int>(type: "int", nullable: false),
            ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            HttpMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
            Url = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            Exceptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Comments = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            HttpStatusCode = table.Column<int>(type: "int", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAuditLogs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcBackgroundJobs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
            JobName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            JobArgs = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: false),
            TryCount = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            NextTryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastTryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            IsAbandoned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            Priority = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)15),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBackgroundJobs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcBlobContainers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBlobContainers", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcClaimTypes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Required = table.Column<bool>(type: "bit", nullable: false),
            IsStatic = table.Column<bool>(type: "bit", nullable: false),
            Regex = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            RegexDescription = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ValueType = table.Column<int>(type: "int", nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcClaimTypes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFeatureGroups",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFeatureGroups", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFeatures",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            DefaultValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
            IsAvailableToHost = table.Column<bool>(type: "bit", nullable: false),
            AllowedProviders = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ValueType = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFeatures", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcFeatureValues",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcFeatureValues", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcLinkUsers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SourceTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TargetTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcLinkUsers", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcOrganizationUnits",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Code = table.Column<string>(type: "nvarchar(95)", maxLength: 95, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            EntityVersion = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcOrganizationUnits", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcOrganizationUnits_EcOrganizationUnits_ParentId",
                      column: x => x.ParentId,
                      principalTable: "EcOrganizationUnits",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcPermissionGrants",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPermissionGrants", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcPermissionGroups",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPermissionGroups", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcPermissions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            MultiTenancySide = table.Column<byte>(type: "tinyint", nullable: false),
            Providers = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            StateCheckers = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcPermissions", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcRoles",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            IsDefault = table.Column<bool>(type: "bit", nullable: false),
            IsStatic = table.Column<bool>(type: "bit", nullable: false),
            IsPublic = table.Column<bool>(type: "bit", nullable: false),
            EntityVersion = table.Column<int>(type: "int", nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcRoles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcSecurityLogs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
            Identity = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
            Action = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcSecurityLogs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcSessions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Device = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            DeviceInfo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            IpAddresses = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
            SignedIn = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcSessions", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcSettingDefinitionRecords",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            DefaultValue = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
            IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
            Providers = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
            IsInherited = table.Column<bool>(type: "bit", nullable: false),
            IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcSettingDefinitionRecords", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            Value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
            ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcTenants",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            NormalizedName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            EntityVersion = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcTenants", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserDelegations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserDelegations", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUsers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            SecurityStamp = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            IsExternal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
            ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "bit", nullable: false),
            EntityVersion = table.Column<int>(type: "int", nullable: false),
            LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
            table.PrimaryKey("PK_EcUsers", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcAuditLogActions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ServiceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            MethodName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            Parameters = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExecutionDuration = table.Column<int>(type: "int", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcAuditLogActions", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcAuditLogActions_EcAuditLogs_AuditLogId",
                      column: x => x.AuditLogId,
                      principalTable: "EcAuditLogs",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcEntityChanges",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            ChangeType = table.Column<byte>(type: "tinyint", nullable: false),
            EntityTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            EntityId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            EntityTypeFullName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcEntityChanges", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcEntityChanges_EcAuditLogs_AuditLogId",
                      column: x => x.AuditLogId,
                      principalTable: "EcAuditLogs",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcBlobs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Content = table.Column<byte[]>(type: "varbinary(max)", maxLength: 2147483647, nullable: true),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcBlobs", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcBlobs_EcBlobContainers_ContainerId",
                      column: x => x.ContainerId,
                      principalTable: "EcBlobContainers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcOrganizationUnitRoles",
          columns: table => new
          {
            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
            table.ForeignKey(
                      name: "FK_EcOrganizationUnitRoles_EcOrganizationUnits_OrganizationUnitId",
                      column: x => x.OrganizationUnitId,
                      principalTable: "EcOrganizationUnits",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EcOrganizationUnitRoles_EcRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "EcRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcRoleClaims",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcRoleClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcRoleClaims_EcRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "EcRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcTenantConnectionStrings",
          columns: table => new
          {
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            Value = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcTenantConnectionStrings", x => new { x.TenantId, x.Name });
            table.ForeignKey(
                      name: "FK_EcTenantConnectionStrings_EcTenants_TenantId",
                      column: x => x.TenantId,
                      principalTable: "EcTenants",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcUserClaims",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcUserClaims_EcUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "EcUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcUserLogins",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ProviderKey = table.Column<string>(type: "nvarchar(196)", maxLength: 196, nullable: false),
            ProviderDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserLogins", x => new { x.UserId, x.LoginProvider });
            table.ForeignKey(
                      name: "FK_EcUserLogins_EcUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "EcUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcUserOrganizationUnits",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
            table.ForeignKey(
                      name: "FK_EcUserOrganizationUnits_EcOrganizationUnits_OrganizationUnitId",
                      column: x => x.OrganizationUnitId,
                      principalTable: "EcOrganizationUnits",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EcUserOrganizationUnits_EcUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "EcUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcUserRoles",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserRoles", x => new { x.UserId, x.RoleId });
            table.ForeignKey(
                      name: "FK_EcUserRoles_EcRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "EcRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_EcUserRoles_EcUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "EcUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcUserTokens",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey(
                      name: "FK_EcUserTokens_EcUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "EcUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcEntityPropertyChanges",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            EntityChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            NewValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            OriginalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            PropertyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            PropertyTypeFullName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcEntityPropertyChanges", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcEntityPropertyChanges_EcEntityChanges_EntityChangeId",
                      column: x => x.EntityChangeId,
                      principalTable: "EcEntityChanges",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcAuditLogActions_AuditLogId",
          table: "EcAuditLogActions",
          column: "AuditLogId");

      migrationBuilder.CreateIndex(
          name: "IX_EcAuditLogActions_TenantId_ServiceName_MethodName_ExecutionTime",
          table: "EcAuditLogActions",
          columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

      migrationBuilder.CreateIndex(
          name: "IX_EcAuditLogs_TenantId_ExecutionTime",
          table: "EcAuditLogs",
          columns: new[] { "TenantId", "ExecutionTime" });

      migrationBuilder.CreateIndex(
          name: "IX_EcAuditLogs_TenantId_UserId_ExecutionTime",
          table: "EcAuditLogs",
          columns: new[] { "TenantId", "UserId", "ExecutionTime" });

      migrationBuilder.CreateIndex(
          name: "IX_EcBackgroundJobs_IsAbandoned_NextTryTime",
          table: "EcBackgroundJobs",
          columns: new[] { "IsAbandoned", "NextTryTime" });

      migrationBuilder.CreateIndex(
          name: "IX_EcBlobContainers_TenantId_Name",
          table: "EcBlobContainers",
          columns: new[] { "TenantId", "Name" });

      migrationBuilder.CreateIndex(
          name: "IX_EcBlobs_ContainerId",
          table: "EcBlobs",
          column: "ContainerId");

      migrationBuilder.CreateIndex(
          name: "IX_EcBlobs_TenantId_ContainerId_Name",
          table: "EcBlobs",
          columns: new[] { "TenantId", "ContainerId", "Name" });

      migrationBuilder.CreateIndex(
          name: "IX_EcEntityChanges_AuditLogId",
          table: "EcEntityChanges",
          column: "AuditLogId");

      migrationBuilder.CreateIndex(
          name: "IX_EcEntityChanges_TenantId_EntityTypeFullName_EntityId",
          table: "EcEntityChanges",
          columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcEntityPropertyChanges_EntityChangeId",
          table: "EcEntityPropertyChanges",
          column: "EntityChangeId");

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatureGroups_Name",
          table: "EcFeatureGroups",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatures_GroupName",
          table: "EcFeatures",
          column: "GroupName");

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatures_Name",
          table: "EcFeatures",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcFeatureValues_Name_ProviderName_ProviderKey",
          table: "EcFeatureValues",
          columns: new[] { "Name", "ProviderName", "ProviderKey" },
          unique: true,
          filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_EcLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId",
          table: "EcLinkUsers",
          columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
          unique: true,
          filter: "[SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_EcOrganizationUnitRoles_RoleId_OrganizationUnitId",
          table: "EcOrganizationUnitRoles",
          columns: new[] { "RoleId", "OrganizationUnitId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcOrganizationUnits_Code",
          table: "EcOrganizationUnits",
          column: "Code");

      migrationBuilder.CreateIndex(
          name: "IX_EcOrganizationUnits_ParentId",
          table: "EcOrganizationUnits",
          column: "ParentId");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
          table: "EcPermissionGrants",
          columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
          unique: true,
          filter: "[TenantId] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissionGroups_Name",
          table: "EcPermissionGroups",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissions_GroupName",
          table: "EcPermissions",
          column: "GroupName");

      migrationBuilder.CreateIndex(
          name: "IX_EcPermissions_Name",
          table: "EcPermissions",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcRoleClaims_RoleId",
          table: "EcRoleClaims",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "IX_EcRoles_NormalizedName",
          table: "EcRoles",
          column: "NormalizedName");

      migrationBuilder.CreateIndex(
          name: "IX_EcSecurityLogs_TenantId_Action",
          table: "EcSecurityLogs",
          columns: new[] { "TenantId", "Action" });

      migrationBuilder.CreateIndex(
          name: "IX_EcSecurityLogs_TenantId_ApplicationName",
          table: "EcSecurityLogs",
          columns: new[] { "TenantId", "ApplicationName" });

      migrationBuilder.CreateIndex(
          name: "IX_EcSecurityLogs_TenantId_Identity",
          table: "EcSecurityLogs",
          columns: new[] { "TenantId", "Identity" });

      migrationBuilder.CreateIndex(
          name: "IX_EcSecurityLogs_TenantId_UserId",
          table: "EcSecurityLogs",
          columns: new[] { "TenantId", "UserId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcSessions_Device",
          table: "EcSessions",
          column: "Device");

      migrationBuilder.CreateIndex(
          name: "IX_EcSessions_SessionId",
          table: "EcSessions",
          column: "SessionId");

      migrationBuilder.CreateIndex(
          name: "IX_EcSessions_TenantId_UserId",
          table: "EcSessions",
          columns: new[] { "TenantId", "UserId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcSettingDefinitionRecords_Name",
          table: "EcSettingDefinitionRecords",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcSettings_Name_ProviderName_ProviderKey",
          table: "EcSettings",
          columns: new[] { "Name", "ProviderName", "ProviderKey" },
          unique: true,
          filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenants_Name",
          table: "EcTenants",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenants_NormalizedName",
          table: "EcTenants",
          column: "NormalizedName");

      migrationBuilder.CreateIndex(
          name: "IX_EcUserClaims_UserId",
          table: "EcUserClaims",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_EcUserLogins_LoginProvider_ProviderKey",
          table: "EcUserLogins",
          columns: new[] { "LoginProvider", "ProviderKey" });

      migrationBuilder.CreateIndex(
          name: "IX_EcUserOrganizationUnits_UserId_OrganizationUnitId",
          table: "EcUserOrganizationUnits",
          columns: new[] { "UserId", "OrganizationUnitId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcUserRoles_RoleId_UserId",
          table: "EcUserRoles",
          columns: new[] { "RoleId", "UserId" });

      migrationBuilder.CreateIndex(
          name: "IX_EcUsers_Email",
          table: "EcUsers",
          column: "Email");

      migrationBuilder.CreateIndex(
          name: "IX_EcUsers_NormalizedEmail",
          table: "EcUsers",
          column: "NormalizedEmail");

      migrationBuilder.CreateIndex(
          name: "IX_EcUsers_NormalizedUserName",
          table: "EcUsers",
          column: "NormalizedUserName");

      migrationBuilder.CreateIndex(
          name: "IX_EcUsers_UserName",
          table: "EcUsers",
          column: "UserName");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcAuditLogActions");

      migrationBuilder.DropTable(
          name: "EcAuditLogExcelFiles");

      migrationBuilder.DropTable(
          name: "EcBackgroundJobs");

      migrationBuilder.DropTable(
          name: "EcBlobs");

      migrationBuilder.DropTable(
          name: "EcClaimTypes");

      migrationBuilder.DropTable(
          name: "EcEntityPropertyChanges");

      migrationBuilder.DropTable(
          name: "EcFeatureGroups");

      migrationBuilder.DropTable(
          name: "EcFeatures");

      migrationBuilder.DropTable(
          name: "EcFeatureValues");

      migrationBuilder.DropTable(
          name: "EcLinkUsers");

      migrationBuilder.DropTable(
          name: "EcOrganizationUnitRoles");

      migrationBuilder.DropTable(
          name: "EcPermissionGrants");

      migrationBuilder.DropTable(
          name: "EcPermissionGroups");

      migrationBuilder.DropTable(
          name: "EcPermissions");

      migrationBuilder.DropTable(
          name: "EcRoleClaims");

      migrationBuilder.DropTable(
          name: "EcSecurityLogs");

      migrationBuilder.DropTable(
          name: "EcSessions");

      migrationBuilder.DropTable(
          name: "EcSettingDefinitionRecords");

      migrationBuilder.DropTable(
          name: "EcSettings");

      migrationBuilder.DropTable(
          name: "EcTenantConnectionStrings");

      migrationBuilder.DropTable(
          name: "EcUserClaims");

      migrationBuilder.DropTable(
          name: "EcUserDelegations");

      migrationBuilder.DropTable(
          name: "EcUserLogins");

      migrationBuilder.DropTable(
          name: "EcUserOrganizationUnits");

      migrationBuilder.DropTable(
          name: "EcUserRoles");

      migrationBuilder.DropTable(
          name: "EcUserTokens");

      migrationBuilder.DropTable(
          name: "EcBlobContainers");

      migrationBuilder.DropTable(
          name: "EcEntityChanges");

      migrationBuilder.DropTable(
          name: "EcTenants");

      migrationBuilder.DropTable(
          name: "EcOrganizationUnits");

      migrationBuilder.DropTable(
          name: "EcRoles");

      migrationBuilder.DropTable(
          name: "EcUsers");

      migrationBuilder.DropTable(
          name: "EcAuditLogs");
    }
  }
}
