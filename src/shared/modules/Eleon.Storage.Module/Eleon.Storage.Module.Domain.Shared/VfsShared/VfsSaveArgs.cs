using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace SharedModule.modules.Blob.Module.VfsShared;
public class VfsSaveArgs : VfsBaseArgs
{
  public Stream BlobStream { get; }

  public bool OverrideExisting { get; }
  public bool IsFolder { get; }
  public VfsSaveArgs(string containerName, string blobName, Stream blobStream, bool overrideExisting = false, bool isFolder = false, CancellationToken cancellationToken = default) 
    : base(containerName, blobName, cancellationToken)
  {
    BlobStream = blobStream;
    OverrideExisting = overrideExisting;
    IsFolder = isFolder;
  }
}
