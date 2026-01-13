using Eleon.Storage.Lib.Models;
using SharedModule.modules.Blob.Module.Constants;
using System;
using System.Collections.Generic;

namespace SharedModule.modules.Blob.Module.Models
{
  public class StorageProviderDto
  {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsTested { get; set; }
    public string? FullType { get; set; }
    public string StorageProviderTypeName { get; set; }
    public IList<StorageProviderSettingDto> Settings { get; set; }
  }
}
