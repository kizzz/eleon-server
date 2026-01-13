
namespace VPortal.SitesManagement.Module.CustomFeatures
{
  public class CustomFeatureGroupDto
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string CategoryName { get; set; }

    public bool IsDynamic { get; set; }
    public string SourceId { get; set; }
  }
}


