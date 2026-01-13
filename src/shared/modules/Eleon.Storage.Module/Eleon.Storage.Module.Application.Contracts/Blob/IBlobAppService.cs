using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Storage.Module.Storage.Module.Application.Contracts.Blob;
public interface IBlobAppService
{
  Task<string> GetAsync(string settingsGroup, string blobName);
  Task<bool> SaveAsync(SaveBlobRequestDto request);
  Task<bool> RemoveAsync(string settingsGroup, string blobName);
}
