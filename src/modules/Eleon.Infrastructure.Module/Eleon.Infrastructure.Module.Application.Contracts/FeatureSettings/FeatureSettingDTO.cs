using System;

namespace VPortal.Core.Infrastructure.Module.FeatureSettings;

public class FeatureSettingDto
{
  public Guid Id { get; set; }
  public Guid? TenantId { get; set; }
  public string Group { get; set; }
  public string Key { get; set; }
  public string Value { get; set; }
  public string Type { get; set; }
  public bool IsEncrypted { get; set; }
  public bool IsRequired { get; set; }
  public DateTime CreationTime { get; set; }
  public DateTime? DeletionTime { get; set; }
  public bool IsDeleted { get; set; }

}
