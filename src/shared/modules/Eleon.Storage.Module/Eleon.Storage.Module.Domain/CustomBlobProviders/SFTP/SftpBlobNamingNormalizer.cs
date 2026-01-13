using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
public class SftpBlobNamingNormalizer : IBlobNamingNormalizer, ITransientDependency
{
  public string NormalizeContainerName(string containerName) => Normalize(containerName);
  public string NormalizeBlobName(string blobName) => Normalize(blobName);

  protected virtual string Normalize(string fileName)
  {
    using (CultureHelper.Use(CultureInfo.InvariantCulture))
    {
      // Remove characters illegal in SFTP paths
      return Regex.Replace(fileName, @"[:*?""<>|]", string.Empty);
    }
  }
}
