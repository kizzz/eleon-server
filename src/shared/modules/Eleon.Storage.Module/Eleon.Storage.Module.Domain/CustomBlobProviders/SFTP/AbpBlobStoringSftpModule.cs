using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace SharedModule.modules.Blob.Module.CustomBlobProviders.SFTP;

[DependsOn(typeof(AbpBlobStoringModule))]
public class AbpBlobStoringSftpModule : AbpModule
{
}
