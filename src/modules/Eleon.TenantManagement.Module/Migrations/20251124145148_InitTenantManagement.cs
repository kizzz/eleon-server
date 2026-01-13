using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.TenantManagement.Module.Migrations
{
  /// <inheritdoc />
  public partial class InitTenantManagement : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcApiKeys",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            RefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Invalidated = table.Column<bool>(type: "bit", nullable: false),
            AllowAuthorize = table.Column<bool>(type: "bit", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            KeySecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcApiKeys", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcControlDelegations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DelegatedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DelegationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            DelegationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            Active = table.Column<bool>(type: "bit", nullable: false),
            Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
            table.PrimaryKey("PK_EcControlDelegations", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcCustomCredentials",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CredentialsSet = table.Column<int>(type: "int", nullable: false),
            Claims = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcCustomCredentials", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcTenantAppearanceSettingEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LightLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LightIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DarkLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DarkIcon = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcTenantAppearanceSettingEntity", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserIsolationSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserIsolationEnabled = table.Column<bool>(type: "bit", nullable: false),
            UserCertificateHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            table.PrimaryKey("PK_EcUserIsolationSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserOtpSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserOtpType = table.Column<int>(type: "int", nullable: false),
            OtpEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
            OtpPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_EcUserOtpSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserSessionStates",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RequirePeriodicPasswordChange = table.Column<bool>(type: "bit", nullable: false),
            PermissionErrorEncountered = table.Column<bool>(type: "bit", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserSessionStates", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcUserSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TwoFaNotificationType = table.Column<int>(type: "int", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcUserSettings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcControlDelegationHistoryEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            ControlDelegationEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcControlDelegationHistoryEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcControlDelegationHistoryEntity_EcControlDelegations_ControlDelegationEntityId",
                      column: x => x.ControlDelegationEntityId,
                      principalTable: "EcControlDelegations",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcTenantSettings",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            TenantIsolationEnabled = table.Column<bool>(type: "bit", nullable: false),
            TenantCertificateHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IpIsolationEnabled = table.Column<bool>(type: "bit", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            AppearanceSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcTenantSettings", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTenantSettings_EcTenantAppearanceSettingEntity_AppearanceSettingsId",
                      column: x => x.AppearanceSettingsId,
                      principalTable: "EcTenantAppearanceSettingEntity",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcTenantContentSecurityHostEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantSettingEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcTenantContentSecurityHostEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTenantContentSecurityHostEntity_EcTenantSettings_TenantSettingEntityId",
                      column: x => x.TenantSettingEntityId,
                      principalTable: "EcTenantSettings",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcTenantExternalLoginProviderEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Type = table.Column<int>(type: "int", nullable: false),
            Enabled = table.Column<bool>(type: "bit", nullable: false),
            Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AdminIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantSettingEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcTenantExternalLoginProviderEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTenantExternalLoginProviderEntity_EcTenantSettings_TenantSettingEntityId",
                      column: x => x.TenantSettingEntityId,
                      principalTable: "EcTenantSettings",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcTenantHostnameEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Subdomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Port = table.Column<int>(type: "int", nullable: false),
            IsSsl = table.Column<bool>(type: "bit", nullable: false),
            ApplicationType = table.Column<int>(type: "int", nullable: false),
            Internal = table.Column<bool>(type: "bit", nullable: false),
            AcceptsClientCertificate = table.Column<bool>(type: "bit", nullable: false),
            Default = table.Column<bool>(type: "bit", nullable: false),
            AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Status = table.Column<int>(type: "int", nullable: false),
            TenantSettingEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcTenantHostnameEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTenantHostnameEntity_EcTenantSettings_TenantSettingEntityId",
                      column: x => x.TenantSettingEntityId,
                      principalTable: "EcTenantSettings",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "EcTenantWhitelistedIpEntity",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            TenantSettingEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
            table.PrimaryKey("PK_EcTenantWhitelistedIpEntity", x => x.Id);
            table.ForeignKey(
                      name: "FK_EcTenantWhitelistedIpEntity_EcTenantSettings_TenantSettingEntityId",
                      column: x => x.TenantSettingEntityId,
                      principalTable: "EcTenantSettings",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcControlDelegationHistoryEntity_ControlDelegationEntityId",
          table: "EcControlDelegationHistoryEntity",
          column: "ControlDelegationEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenantContentSecurityHostEntity_TenantSettingEntityId",
          table: "EcTenantContentSecurityHostEntity",
          column: "TenantSettingEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenantExternalLoginProviderEntity_TenantSettingEntityId",
          table: "EcTenantExternalLoginProviderEntity",
          column: "TenantSettingEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenantHostnameEntity_TenantSettingEntityId",
          table: "EcTenantHostnameEntity",
          column: "TenantSettingEntityId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenantSettings_AppearanceSettingsId",
          table: "EcTenantSettings",
          column: "AppearanceSettingsId");

      migrationBuilder.CreateIndex(
          name: "IX_EcTenantWhitelistedIpEntity_TenantSettingEntityId",
          table: "EcTenantWhitelistedIpEntity",
          column: "TenantSettingEntityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcApiKeys");

      migrationBuilder.DropTable(
          name: "EcControlDelegationHistoryEntity");

      migrationBuilder.DropTable(
          name: "EcCustomCredentials");

      migrationBuilder.DropTable(
          name: "EcTenantContentSecurityHostEntity");

      migrationBuilder.DropTable(
          name: "EcTenantExternalLoginProviderEntity");

      migrationBuilder.DropTable(
          name: "EcTenantHostnameEntity");

      migrationBuilder.DropTable(
          name: "EcTenantWhitelistedIpEntity");

      migrationBuilder.DropTable(
          name: "EcUserIsolationSettings");

      migrationBuilder.DropTable(
          name: "EcUserOtpSettings");

      migrationBuilder.DropTable(
          name: "EcUserSessionStates");

      migrationBuilder.DropTable(
          name: "EcUserSettings");

      migrationBuilder.DropTable(
          name: "EcControlDelegations");

      migrationBuilder.DropTable(
          name: "EcTenantSettings");

      migrationBuilder.DropTable(
          name: "EcTenantAppearanceSettingEntity");
    }
  }
}
