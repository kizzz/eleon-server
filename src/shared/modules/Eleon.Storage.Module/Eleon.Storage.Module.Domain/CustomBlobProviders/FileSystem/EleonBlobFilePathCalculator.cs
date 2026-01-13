using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Storage.Module.Helpers;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.MultiTenancy;

namespace Eleon.Storage.Lib.CustomBlobProviders.FileSystem
{
  public class EleonBlobFilePathCalculator : DefaultBlobFilePathCalculator
  {
    public EleonBlobFilePathCalculator() : base(null)
    {
    }

    public override string Calculate(BlobProviderArgs args)
    {
      var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
      var tenantId = TryGetConfiguration(args, "TenantId");
      var isMultitenancyEnabled = TryGetConfiguration(args, "FileSystem.IsMultitenancyEnabled");
      var appendContainerNameToBasePath = TryGetConfiguration(args, "FileSystem.AppendContainerNameToBasePath");
      var blobPath = fileSystemConfiguration.BasePath;

      if (IsTrue(isMultitenancyEnabled))
      {
        if (tenantId == null)
        {
          blobPath = Path.Combine(blobPath, "host");
        }
        else
        {
          blobPath = Path.Combine(blobPath, "tenants", tenantId);
        }
      }

      if (IsTrue(appendContainerNameToBasePath))
      {
        blobPath = Path.Combine(blobPath, args.ContainerName);
      }

      // Validate and normalize path to prevent directory traversal attacks
      return PathSecurityValidator.NormalizeAndValidate(blobPath, args.BlobName);
    }

    private static string? TryGetConfiguration(BlobProviderArgs args, string key)
    {
      try
      {
        return args.Configuration.GetConfiguration<string>(key);
      }
      catch (AbpException)
      {
        return null;
      }
    }

    private static bool IsTrue(string? value)
      => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
  }
}
