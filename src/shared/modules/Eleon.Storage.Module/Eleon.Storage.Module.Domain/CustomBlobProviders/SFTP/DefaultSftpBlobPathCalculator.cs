using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Storage.Module.Helpers;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
public class DefaultSftpBlobPathCalculator : IBlobSftpPathCalculator
{
  public DefaultSftpBlobPathCalculator()
  {
  }

  public virtual string Calculate(BlobProviderArgs args)
  {
    var cfg = args.Configuration.GetSftpConfiguration();
    var path = cfg.BasePath;
    var tenantId = args.Configuration.GetConfiguration<string>("TenantId");
    var isMultitenancyEnabled = args.Configuration.GetConfiguration<string>("Sftp.IsMultitenancyEnabled");
    var appendContainerNameToBasePath = args.Configuration.GetConfiguration<string>("Sftp.AppendContainerNameToBasePath");

    if (isMultitenancyEnabled == "true")
    {
      path = tenantId == null
          ? Path.Combine(path, "host")
          : Path.Combine(path, "tenants", tenantId);
    }

    if (appendContainerNameToBasePath == "true")
      path = Path.Combine(path, args.ContainerName);

    // Normalize to forward slashes for remote paths
    path = path.Replace("\\", "/");

    // Validate and normalize remote path to prevent directory traversal attacks
    return PathSecurityValidator.NormalizeAndValidateRemote(path, args.BlobName);
  }
}
