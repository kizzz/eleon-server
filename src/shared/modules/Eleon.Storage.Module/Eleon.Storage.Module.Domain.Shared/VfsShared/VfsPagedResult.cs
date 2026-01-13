using SharedModule.modules.Blob.Module.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared
{
  public sealed record VfsPagedResult(IReadOnlyList<VfsFileInfo> Items, long TotalCount);
}
