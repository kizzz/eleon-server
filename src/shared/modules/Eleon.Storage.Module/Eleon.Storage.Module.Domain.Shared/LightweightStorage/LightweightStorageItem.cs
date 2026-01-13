using System.Collections.Generic;

namespace Storage.Module.LightweightStorage
{
  public class LightweightStorageItem
  {
    public List<string> Permissions { get; set; }
    public string Base64 { get; set; }
  }
}
