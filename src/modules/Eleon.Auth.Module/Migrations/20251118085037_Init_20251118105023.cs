using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleon.Auth.Module.Migrations
{
  /// <inheritdoc />
  public partial class Init_20251118105023 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiResources",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            Enabled = table.Column<bool>(type: "bit", nullable: false),
            AllowedAccessTokenSigningAlgorithms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            ShowInDiscoveryDocument = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcIdentityServerApiResources", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiScopes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Enabled = table.Column<bool>(type: "bit", nullable: false),
            Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            Required = table.Column<bool>(type: "bit", nullable: false),
            Emphasize = table.Column<bool>(type: "bit", nullable: false),
            ShowInDiscoveryDocument = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcIdentityServerApiScopes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClients",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            ClientUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            LogoUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            Enabled = table.Column<bool>(type: "bit", nullable: false),
            ProtocolType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            RequireClientSecret = table.Column<bool>(type: "bit", nullable: false),
            RequireConsent = table.Column<bool>(type: "bit", nullable: false),
            AllowRememberConsent = table.Column<bool>(type: "bit", nullable: false),
            AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(type: "bit", nullable: false),
            RequirePkce = table.Column<bool>(type: "bit", nullable: false),
            AllowPlainTextPkce = table.Column<bool>(type: "bit", nullable: false),
            RequireRequestObject = table.Column<bool>(type: "bit", nullable: false),
            AllowAccessTokensViaBrowser = table.Column<bool>(type: "bit", nullable: false),
            FrontChannelLogoutUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            FrontChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
            BackChannelLogoutUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            BackChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
            AllowOfflineAccess = table.Column<bool>(type: "bit", nullable: false),
            IdentityTokenLifetime = table.Column<int>(type: "int", nullable: false),
            AllowedIdentityTokenSigningAlgorithms = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            AccessTokenLifetime = table.Column<int>(type: "int", nullable: false),
            AuthorizationCodeLifetime = table.Column<int>(type: "int", nullable: false),
            ConsentLifetime = table.Column<int>(type: "int", nullable: true),
            AbsoluteRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
            SlidingRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
            RefreshTokenUsage = table.Column<int>(type: "int", nullable: false),
            UpdateAccessTokenClaimsOnRefresh = table.Column<bool>(type: "bit", nullable: false),
            RefreshTokenExpiration = table.Column<int>(type: "int", nullable: false),
            AccessTokenType = table.Column<int>(type: "int", nullable: false),
            EnableLocalLogin = table.Column<bool>(type: "bit", nullable: false),
            IncludeJwtId = table.Column<bool>(type: "bit", nullable: false),
            AlwaysSendClientClaims = table.Column<bool>(type: "bit", nullable: false),
            ClientClaimsPrefix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            PairWiseSubjectSalt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            UserSsoLifetime = table.Column<int>(type: "int", nullable: true),
            UserCodeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            DeviceCodeLifetime = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_EcIdentityServerClients", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerDeviceFlowCodes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DeviceCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            UserCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
            Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerDeviceFlowCodes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerIdentityResources",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            Enabled = table.Column<bool>(type: "bit", nullable: false),
            Required = table.Column<bool>(type: "bit", nullable: false),
            Emphasize = table.Column<bool>(type: "bit", nullable: false),
            ShowInDiscoveryDocument = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_EcIdentityServerIdentityResources", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerPersistedGrants",
          columns: table => new
          {
            Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
            ConsumedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerPersistedGrants", x => x.Key);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiResourceClaims",
          columns: table => new
          {
            Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            ApiResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiResourceClaims", x => new { x.ApiResourceId, x.Type });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiResourceClaims_EcIdentityServerApiResources_ApiResourceId",
                      column: x => x.ApiResourceId,
                      principalTable: "EcIdentityServerApiResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiResourceProperties",
          columns: table => new
          {
            ApiResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiResourceProperties", x => new { x.ApiResourceId, x.Key, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiResourceProperties_EcIdentityServerApiResources_ApiResourceId",
                      column: x => x.ApiResourceId,
                      principalTable: "EcIdentityServerApiResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiResourceScopes",
          columns: table => new
          {
            ApiResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Scope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiResourceScopes", x => new { x.ApiResourceId, x.Scope });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiResourceScopes_EcIdentityServerApiResources_ApiResourceId",
                      column: x => x.ApiResourceId,
                      principalTable: "EcIdentityServerApiResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiResourceSecrets",
          columns: table => new
          {
            Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
            ApiResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            Expiration = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiResourceSecrets", x => new { x.ApiResourceId, x.Type, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiResourceSecrets_EcIdentityServerApiResources_ApiResourceId",
                      column: x => x.ApiResourceId,
                      principalTable: "EcIdentityServerApiResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiScopeClaims",
          columns: table => new
          {
            Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            ApiScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiScopeClaims", x => new { x.ApiScopeId, x.Type });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiScopeClaims_EcIdentityServerApiScopes_ApiScopeId",
                      column: x => x.ApiScopeId,
                      principalTable: "EcIdentityServerApiScopes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerApiScopeProperties",
          columns: table => new
          {
            ApiScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerApiScopeProperties", x => new { x.ApiScopeId, x.Key, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerApiScopeProperties_EcIdentityServerApiScopes_ApiScopeId",
                      column: x => x.ApiScopeId,
                      principalTable: "EcIdentityServerApiScopes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientClaims",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientClaims", x => new { x.ClientId, x.Type, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientClaims_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientCorsOrigins",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Origin = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientCorsOrigins", x => new { x.ClientId, x.Origin });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientCorsOrigins_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientGrantTypes",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            GrantType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientGrantTypes", x => new { x.ClientId, x.GrantType });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientGrantTypes_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientIdPRestrictions",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Provider = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientIdPRestrictions", x => new { x.ClientId, x.Provider });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientIdPRestrictions_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientPostLogoutRedirectUris",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PostLogoutRedirectUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientPostLogoutRedirectUris", x => new { x.ClientId, x.PostLogoutRedirectUri });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientPostLogoutRedirectUris_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientProperties",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientProperties", x => new { x.ClientId, x.Key, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientProperties_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientRedirectUris",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RedirectUri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientRedirectUris", x => new { x.ClientId, x.RedirectUri });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientRedirectUris_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientScopes",
          columns: table => new
          {
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Scope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientScopes", x => new { x.ClientId, x.Scope });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientScopes_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerClientSecrets",
          columns: table => new
          {
            Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
            ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            Expiration = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerClientSecrets", x => new { x.ClientId, x.Type, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerClientSecrets_EcIdentityServerClients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "EcIdentityServerClients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerIdentityResourceClaims",
          columns: table => new
          {
            Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            IdentityResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerIdentityResourceClaims", x => new { x.IdentityResourceId, x.Type });
            table.ForeignKey(
                      name: "FK_EcIdentityServerIdentityResourceClaims_EcIdentityServerIdentityResources_IdentityResourceId",
                      column: x => x.IdentityResourceId,
                      principalTable: "EcIdentityServerIdentityResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "EcIdentityServerIdentityResourceProperties",
          columns: table => new
          {
            IdentityResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_EcIdentityServerIdentityResourceProperties", x => new { x.IdentityResourceId, x.Key, x.Value });
            table.ForeignKey(
                      name: "FK_EcIdentityServerIdentityResourceProperties_EcIdentityServerIdentityResources_IdentityResourceId",
                      column: x => x.IdentityResourceId,
                      principalTable: "EcIdentityServerIdentityResources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerClients_ClientId",
          table: "EcIdentityServerClients",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerDeviceFlowCodes_DeviceCode",
          table: "EcIdentityServerDeviceFlowCodes",
          column: "DeviceCode",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerDeviceFlowCodes_Expiration",
          table: "EcIdentityServerDeviceFlowCodes",
          column: "Expiration");

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerDeviceFlowCodes_UserCode",
          table: "EcIdentityServerDeviceFlowCodes",
          column: "UserCode");

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerPersistedGrants_Expiration",
          table: "EcIdentityServerPersistedGrants",
          column: "Expiration");

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerPersistedGrants_SubjectId_ClientId_Type",
          table: "EcIdentityServerPersistedGrants",
          columns: new[] { "SubjectId", "ClientId", "Type" });

      migrationBuilder.CreateIndex(
          name: "IX_EcIdentityServerPersistedGrants_SubjectId_SessionId_Type",
          table: "EcIdentityServerPersistedGrants",
          columns: new[] { "SubjectId", "SessionId", "Type" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "EcIdentityServerApiResourceClaims");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiResourceProperties");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiResourceScopes");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiResourceSecrets");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiScopeClaims");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiScopeProperties");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientClaims");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientCorsOrigins");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientGrantTypes");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientIdPRestrictions");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientPostLogoutRedirectUris");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientProperties");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientRedirectUris");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientScopes");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClientSecrets");

      migrationBuilder.DropTable(
          name: "EcIdentityServerDeviceFlowCodes");

      migrationBuilder.DropTable(
          name: "EcIdentityServerIdentityResourceClaims");

      migrationBuilder.DropTable(
          name: "EcIdentityServerIdentityResourceProperties");

      migrationBuilder.DropTable(
          name: "EcIdentityServerPersistedGrants");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiResources");

      migrationBuilder.DropTable(
          name: "EcIdentityServerApiScopes");

      migrationBuilder.DropTable(
          name: "EcIdentityServerClients");

      migrationBuilder.DropTable(
          name: "EcIdentityServerIdentityResources");
    }
  }
}
