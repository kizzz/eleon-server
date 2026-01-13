using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
public interface IBlobSftpPathCalculator
{
  string Calculate(BlobProviderArgs args);
}
