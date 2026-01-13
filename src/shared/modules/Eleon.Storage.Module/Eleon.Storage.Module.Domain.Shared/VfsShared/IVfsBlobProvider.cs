using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace SharedModule.modules.Blob.Module.Shared;
public interface IVfsBlobProvider : IDisposable
{
  Task<IReadOnlyList<VfsFileInfo>> ListAsync(VfsListArgs args);
  Task SaveAsync(VfsSaveArgs args);

  Task<bool> DeleteAsync(VfsDeleteArgs args);

  Task<bool> ExistsAsync(VfsExistArgs args);
  Task<bool> TestAsync(VfsTestArgs args);

  Task<Stream?> GetOrNullAsync(VfsGetArgs args);
  Task<byte[]?> GetAllBytesOrNullAsync(VfsGetArgs args);
  Task<VfsPagedResult> ListPagedAsync(VfsListPagedArgs args);
}
