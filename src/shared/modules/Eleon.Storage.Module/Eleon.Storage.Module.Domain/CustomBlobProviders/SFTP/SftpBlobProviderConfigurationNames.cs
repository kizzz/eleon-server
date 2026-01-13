using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
public static class SftpBlobProviderConfigurationNames
{
  public const string Host = "Sftp.Host";
  public const string Port = "Sftp.Port";
  public const string Username = "Sftp.UserName";
  public const string Password = "Sftp.Password";
  public const string BasePath = "Sftp.BasePath";
  public const string AppendContainerNameToBasePath = "Sftp.AppendContainerNameToBasePath";
  public const string UsePrivateKey = "Sftp.UsePrivateKey";
  public const string PrivateKey = "Sftp.PrivateKey";
}
