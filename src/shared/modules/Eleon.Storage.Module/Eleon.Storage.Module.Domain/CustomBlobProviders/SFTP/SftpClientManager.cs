using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Storage.Lib.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.Extensions.Msal;
using Org.BouncyCastle.Asn1.X509;
using Renci.SshNet;
using SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using VPortal.Storage.Module;
using Volo.Abp.DependencyInjection;
namespace SharedModule.modules.Blob.Module.CustomBlobProviders.SFTP;

public static class SftpClientManager
{
  private static readonly ConcurrentDictionary<string, SftpClient> _clients = new();

  private static string GetCacheKey(SftpBlobProviderConfiguration config, string tenantId)
  {
    return $"{tenantId}:{config.Host}:{config.Port}:{config.Username}";
  }
  private static void LogConfiguration(SftpBlobProviderConfiguration config)
  {
    // Redact sensitive information: passwords, private keys, and key paths
    var passwordDisplay = string.IsNullOrWhiteSpace(config.Password) 
        ? "[not set]" 
        : StorageDomainConsts.CensoredValue;
    
    var privateKeyDisplay = string.IsNullOrWhiteSpace(config.PrivateKey)
        ? "[not set]"
        : StorageDomainConsts.CensoredValue;
    
    var usePrivateKey = bool.TryParse(config.UsePrivateKey, out var parsedUseKey) && parsedUseKey;

    EleonsoftLog.Info($"SFTP VFS Client Configuration - Host: {config.Host}, Port: {config.Port}, Username: {config.Username}, BasePath: {config.BasePath}, Password: {passwordDisplay}, UsePrivateKey: {usePrivateKey}, PrivateKey: {privateKeyDisplay}");
  }

  public static SftpClient GetOrCreateClient(SftpBlobProviderConfiguration config, string tenantId)
  {
    LogConfiguration(config);
    var key = GetCacheKey(config, tenantId);

    var port = int.TryParse(config.Port, out var parsedPort) ? parsedPort : 22;
    var usePrivateKey = bool.TryParse(config.UsePrivateKey, out var parsedUseKey) && parsedUseKey;

    SftpClient CreateAndConnect()
    {
      var client = CreateClient(config, port, usePrivateKey);
      client.Connect();
      client.ChangeDirectory(config.BasePath);
      return client;
    }

    var client = _clients.GetOrAdd(key, _ => CreateAndConnect());

    if (!client.IsConnected)
    {
      try
      {
        client.Connect();
        client.ChangeDirectory(config.BasePath);
      }
      catch
      {
        _clients.TryRemove(key, out _);
        client.Dispose();

        client = CreateAndConnect();
        _clients[key] = client;
      }
    }

    return client;
  }

  private static SftpClient CreateClient(SftpBlobProviderConfiguration config, int port, bool usePrivateKey)
  {
    // If key authentication is requested, use the raw private key text converted to a temp file in memory.
    string tempKeyPath = null;

    if (usePrivateKey)
    {
      if (string.IsNullOrWhiteSpace(config.PrivateKey))
        throw new ArgumentException("Private key is required when UsePrivateKey is true.");

      // Write the in-memory private key into a temp file
      tempKeyPath = Path.GetTempFileName();
      File.WriteAllText(tempKeyPath, config.PrivateKey);
    }

    var client = SFTPHelper.GetSFTPClient(
        server: config.Host,
        port: port,
        user: config.Username,
        password: config.Password,
        loadFolder: null,
        keyPath: tempKeyPath,
        keyPassphrase: null,
        useKeyAuthentication: usePrivateKey,
        filesFilter: "*"
    );

    return client;
  }

  public static void Dispose()
  {
    foreach (var client in _clients.Values)
    {
      try { client.Disconnect(); client.Dispose(); } catch { }
    }

    _clients.Clear();
  }
}
