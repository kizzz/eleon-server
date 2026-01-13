using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module.DomainServices
{

  public class DatabaseDomainService : DomainService
  {
    private readonly IVportalLogger<DatabaseDomainService> logger;
    private readonly IConfiguration configuration;
    private readonly IStringLocalizer<SitesManagementResource> localizer;
    private readonly UnitOfWorkManager unitOfWorkManager;

    public DatabaseDomainService(
        IVportalLogger<DatabaseDomainService> logger,
        IConfiguration configuration,
        IStringLocalizer<SitesManagementResource> localizer,
        UnitOfWorkManager unitOfWorkManager)
    {
      this.logger = logger;
      this.configuration = configuration;
      this.localizer = localizer;
      this.unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<string> CreateDatabaseBasedOnDefault(string dbName, string userName, string userPass)
    {
      string newConnectionString = null;
      try
      {
        using var masterConnection = OpenMasterSqlConnection();
        await EnsureDatabaseCanBeCreated(masterConnection, dbName, userName);

        CreateDatabase(masterConnection, dbName);
        CreateDatabaseLogin(masterConnection, dbName, userName, userPass);
        EnableDatabaseReadCommitSnapshot(masterConnection, dbName);

        string defaultConnectionString = GetDefaultConnectionString();
        newConnectionString = CreateConnectionString(defaultConnectionString, dbName, userName, userPass);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return newConnectionString;
    }

    internal async Task EnsureDatabaseCanBeCreated(string dbName, string userName)
    {
      try
      {
        using var masterConnection = OpenMasterSqlConnection();
        await EnsureDatabaseCanBeCreated(masterConnection, dbName, userName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private string CreateConnectionString(string baseConnectionString, string dbName, string userName, string userPass)
    {
      var builder = new SqlConnectionStringBuilder(baseConnectionString)
      {
        InitialCatalog = dbName,
        UserID = userName,
        Password = userPass,
      };

      return builder.ToString();
    }

    private void CreateDatabaseLogin(SqlConnection connection, string dbName, string userName, string userPass)
    {
      string createUserSql = @$"
                USE [{dbName}];
                CREATE LOGIN {userName} WITH PASSWORD = '{userPass}';
                CREATE USER {userName} FOR LOGIN {userName};
                ALTER ROLE db_owner ADD MEMBER {userName};";

      using var createUserCommand = new SqlCommand(createUserSql, connection);
      createUserCommand.ExecuteNonQuery();
    }

    private void EnableDatabaseReadCommitSnapshot(SqlConnection connection, string dbName)
    {
      using var alterDbCommand = new SqlCommand($"ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT ON;", connection);
      alterDbCommand.ExecuteNonQuery();
    }

    private void CreateDatabase(SqlConnection connection, string dbName)
    {
      using var createDbCommand = new SqlCommand($"CREATE DATABASE [{dbName}];", connection);
      createDbCommand.ExecuteNonQuery();
    }

    private async Task EnsureDatabaseCanBeCreated(SqlConnection connection, string dbName, string userName)
    {
      EnsureDatabaseNameAvailable(dbName, connection);
      EnsureDatabaseLoginAvailable(userName, connection);
    }

    private SqlConnection OpenMasterSqlConnection()
    {
      string masterConnectionString = GetDefaultConnectionString();
      var masterConnection = new SqlConnection(masterConnectionString);
      masterConnection.Open();
      return masterConnection;
    }

    private string GetDefaultConnectionString()
    {
      var connectionString = configuration.GetConnectionString("Default");
      if (string.IsNullOrEmpty(connectionString))
      {
        throw new Exception("Connection string not found");
      }

      return connectionString;
    }

    private void EnsureDatabaseNameAvailable(string dbName, SqlConnection connection)
    {
      using var checkDBCommand = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName}'", connection);
      bool dbExists = (int)checkDBCommand.ExecuteScalar() > 0;
      if (dbExists)
      {
        throw new UserFriendlyException(localizer["CreateDatabase:Error:DatabaseNameTaken", dbName]);
      }
    }

    private void EnsureDatabaseLoginAvailable(string userName, SqlConnection connection)
    {
      using var checkUserCommand = new SqlCommand($"SELECT COUNT(*) FROM master.sys.sql_logins WHERE name = '{userName}'", connection);
      bool userExists = (int)checkUserCommand.ExecuteScalar() > 0;
      if (userExists)
      {
        throw new UserFriendlyException(localizer["CreateDatabase:Error:DatabaseLoginTaken", userName]);
      }
    }
  }
}

