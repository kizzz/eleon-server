using Eleon.Storage.Lib.Constants;
using Eleon.Storage.Lib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModule.modules.Blob.Module.Models;

public class StorageProviderSettingDto
{
  public Guid Id { get; set; }
  public Guid StorageProviderId { get; set; }
  public string? Value { get; set; }
  public string Key { get; set; }
}
