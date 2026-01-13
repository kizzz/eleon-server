using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Storage.Module.Storage.Module.Application.Contracts.Blob;
public class SaveBlobRequestDto
{
  public string SettingGroup { get; set; }
  public string BlobName { get; set; }
  public string FileBase64 { get; set; }
}
