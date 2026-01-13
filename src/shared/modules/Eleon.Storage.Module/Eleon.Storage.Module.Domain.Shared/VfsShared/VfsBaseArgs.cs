using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedModule.modules.Blob.Module.VfsShared;
public class VfsBaseArgs
{
  public string ContainerName { get; set; }
  public string BlobName { get; }
  public CancellationToken CancellationToken { get; }

  public VfsBaseArgs(string containerName, string blobName, CancellationToken cancellationToken = default)
  {
    ContainerName = containerName;
    BlobName = blobName;
    CancellationToken = cancellationToken;
  }
}
