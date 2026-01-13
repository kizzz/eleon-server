using Volo.Abp.MultiTenancy;

namespace Common.Module.Permissions
{
  public class PermissionDescription
  {
    public string Name { get; set; }
    public string LocalizationKey { get; set; }
    public string DisplayName { get; set; }
    public MultiTenancySides MultiTenancySide { get; set; } = MultiTenancySides.Both;
    public List<PermissionDescription> Children { get; set; }

    public PermissionDescription(string name, string key, List<PermissionDescription> children = null)
    {
      Name = name;
      LocalizationKey = key;
      Children = children;
    }

    public PermissionDescription()
    {
    }
  }

  public class PermissionGroupDescription
  {
    public string Name { get; set; }
    public string LocalizationKey { get; set; }
    public string DisplayName { get; set; }
    public List<PermissionDescription> Children { get; set; }

    public PermissionGroupDescription(string name, string key, List<PermissionDescription> children)
    {
      Name = name;
      LocalizationKey = key;
      Children = children;
    }

    public PermissionGroupDescription()
    {
    }
  }

  public class FeatureGroupDescription
  {
    public string Name { get; set; }
    public string LocalizationKey { get; set; }
    public string DisplayName { get; set; }
    public List<FeatureDescription> Children { get; set; }

    public FeatureGroupDescription(string name, string key, List<FeatureDescription> children)
    {
      Name = name;
      LocalizationKey = key;
      Children = children;
    }

    public FeatureGroupDescription()
    {
    }
  }

  public class FeatureDescription
  {
    public string Name { get; set; }
    public string LocalizationKey { get; set; }
    public string DefaultValue { get; set; }
    public FeatureValueType Type { get; set; }
    public string DisplayName { get; set; }
    public List<PermissionGroupDescription> Permissions { get; set; }

    public FeatureDescription(string name, string key, FeatureValueType type, string defaultValue, List<PermissionGroupDescription> permissions)
    {
      Name = name;
      LocalizationKey = key;
      Type = type;
      Permissions = permissions;
      DefaultValue = defaultValue;
    }

    public FeatureDescription()
    {
    }
  }

  public enum FeatureValueType
  {
    Toggle,
    Text,
    Number,
  }
}
