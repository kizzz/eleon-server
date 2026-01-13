using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Blob.Module.Shared;
public class VfsFileInfo
{
  public string Key { get; set; }
  public long Size { get; set; }
  public bool IsFolder { get; set; }
  public DateTime? LastModified { get; set; }
}
