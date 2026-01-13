using Common.Module.Helpers;
using System;
using System.Collections.Generic;

namespace Storage.Module.LightweightStorage
{
  public class LightweightStorageOptions
  {
    public SizeUnits MaxSizeUnit { get; set; } = SizeUnits.B;
    public long MaxSize { get; set; } = 0;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromDays(1);
    public List<string> RequiredPermissions = [];
  }
}
