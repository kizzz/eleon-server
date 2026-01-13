using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Storage.Lib.Models;
using Renci.SshNet;
using SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;


namespace Eleon.Storage.Lib.Helpers;
public static class SFTPHelper
{
  public static SftpClient GetSFTPClient(
  string server,
  int port = 22,
  string user = null,
  string password = null,
  string loadFolder = null,
  string keyPath = null,
  string keyPassphrase = null,
  bool useKeyAuthentication = false,
  string filesFilter = "*"
)
  {
    if (string.IsNullOrWhiteSpace(server))
      throw new ArgumentNullException(nameof(server));

    if (string.IsNullOrWhiteSpace(user))
      throw new ArgumentNullException(nameof(user));

    SftpClient client = null;

    var hasKeyPath = !string.IsNullOrWhiteSpace(keyPath) && keyPath != "-";
    var hasPassword = !string.IsNullOrWhiteSpace(password) && password != "-";

    if (useKeyAuthentication)
    {
      if (!hasKeyPath)
        throw new Exception("SFTP settings not valid: key authentication is enabled but no key path provided.");

      var resolvedKeyPath = keyPath;

      var authMethods = new List<AuthenticationMethod>
        {
            string.IsNullOrWhiteSpace(keyPassphrase)
                ? new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(resolvedKeyPath))
                : new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(resolvedKeyPath, keyPassphrase))
        };

      // Optional combined key + password auth
      if (hasPassword)
      {
        authMethods.Add(new PasswordAuthenticationMethod(user, password));
      }

      var connectionInfo = new ConnectionInfo(server, port, user, authMethods.ToArray());
      client = new SftpClient(connectionInfo);
    }
    else if (hasPassword)
    {
      client = new SftpClient(server, port, user, password);
    }
    else
    {
      throw new Exception("SFTP settings not valid: no valid password or key provided.");
    }

    return client;
  }
  private static void LogConfiguration(SftpOptions config)
  {
    EleonsoftLog.Info($"SFTP datasource Client Configuration - Host: {config.Server}, Port: {config.Port}, Username: {config.User}, BasePath: {config.LoadFolder}, Password: {config.Password}, file: {config.KeyPath}");
  }


  public static SftpClient GetSFTPClient(SftpOptions config)
  {
    var keyPath = config.KeyPath;

    if (!string.IsNullOrWhiteSpace(keyPath) && !Path.IsPathRooted(keyPath))
    {
      keyPath = Path.GetDirectoryName(AppContext.BaseDirectory) + keyPath;
    }
    LogConfiguration(config);
    return GetSFTPClient(config.Server,
        config.Port,
        config.User,
        config.Password,
        config.LoadFolder,
        keyPath,
        config.KeyPassphrase,
        config.UseKeyAuthentication,
        config.FilesFilter);
  }
}
