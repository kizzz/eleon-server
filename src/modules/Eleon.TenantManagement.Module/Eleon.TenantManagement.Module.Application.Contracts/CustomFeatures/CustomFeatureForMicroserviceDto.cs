using VPortal.TenantManagement.Module.CustomFeatures;

namespace VPortal.TenantManagement.Module.Application.Contracts.CustomFeatures
{
  public class CustomFeatureForMicroserviceDto
  {
    public string SourceId { get; set; }
    public List<CustomFeatureGroupDto> Groups { get; set; }
    public List<CustomFeatureDto> Features { get; set; }
  }
}
