using Eleon.Storage.Lib.Models;
using SharedModule.modules.Blob.Module.Constants;
using System;
using System.Collections.Generic;

namespace SharedModule.modules.Blob.Module.Models
{
  public class MinimalStorageProviderDto
  {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
  }
}
