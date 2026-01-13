using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

using System.Threading;

namespace SharedModule.modules.Blob.Module.Shared;
public class VfsListPagedArgs : VfsBaseArgs
{
  public bool IsRecursiveSearch { get; set; }
  public int MaxResults { get; set; } = 100;
  public int SkipResults { get; set; } = 0;
  public VfsListPagedArgs(string containerName, string prefix, bool isRecursiveSearch = false, CancellationToken cancellationToken = default) 
    : base(containerName, prefix, cancellationToken)
  {
    IsRecursiveSearch = isRecursiveSearch;
  }
}
