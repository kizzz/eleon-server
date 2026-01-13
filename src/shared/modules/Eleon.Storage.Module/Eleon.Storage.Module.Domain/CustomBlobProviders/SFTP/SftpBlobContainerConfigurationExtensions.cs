using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
public static class SftpBlobContainerConfigurationExtensions
{
  public static SftpBlobProviderConfiguration GetSftpConfiguration(
      this BlobContainerConfiguration containerConfiguration)
  {
    return new SftpBlobProviderConfiguration(containerConfiguration);
  }

  public static BlobContainerConfiguration UseSftp(
      this BlobContainerConfiguration containerConfiguration,
      Action<SftpBlobProviderConfiguration> sftpConfigureAction)
  {
    containerConfiguration.ProviderType = typeof(SftpBlobProvider);
    containerConfiguration.NamingNormalizers.TryAdd<SftpBlobNamingNormalizer>();
    sftpConfigureAction(new SftpBlobProviderConfiguration(containerConfiguration));
    return containerConfiguration;
  }
}
