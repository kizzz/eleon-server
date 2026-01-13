using SharedModule.modules.Blob.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Application.Contracts.StorageProviders
{
  public class CreateStorageProviderDto
  {
    public string Name { get; set; }
    public string TypeName { get; set; }
  }
}
