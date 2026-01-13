using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;


public class SftpBlobProviderConfiguration
{
  private readonly BlobContainerConfiguration _containerConfiguration;

  public string Host
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.Host);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.Host, value);
  }

  public string Port
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.Port);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.Port, value);
  }

  public string Username
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.Username);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.Username, value);
  }

  public string Password
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.Password);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.Password, value);
  }

  public string BasePath
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.BasePath);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.BasePath, value);
  }

  public bool AppendContainerNameToBasePath
  {
    get => _containerConfiguration.GetConfigurationOrDefault(SftpBlobProviderConfigurationNames.AppendContainerNameToBasePath, true);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.AppendContainerNameToBasePath, value);
  }
  public string UsePrivateKey
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.UsePrivateKey);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.UsePrivateKey, value);
  }
  public string PrivateKey
  {
    get => _containerConfiguration.GetConfiguration<string>(SftpBlobProviderConfigurationNames.PrivateKey);
    set => _containerConfiguration.SetConfiguration(SftpBlobProviderConfigurationNames.PrivateKey, value);
  }

  public SftpBlobProviderConfiguration(BlobContainerConfiguration containerConfiguration)
  {
    _containerConfiguration = containerConfiguration;
  }
}
