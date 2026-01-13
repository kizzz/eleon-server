using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace SharedModule.modules.Blob.Module.VfsShared;
public class VfsExistArgs : VfsBaseArgs
{
  public VfsExistArgs(string containerName, string blobName, CancellationToken cancellationToken = default) 
    : base(containerName, blobName, cancellationToken)
  {
  }
}
