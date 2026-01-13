namespace Messaging.Module.ETO.Google
{
  public class RouteShipmentTypeRequirementDto
  {
    public List<string> DependentShipmentTypes { get; set; } = new List<string>();
    public List<string> RequiredShipmentTypeAlternatives { get; set; } = new List<string>();
    public RouteShipmentTypeRequirementMode RequirementMode { get; set; }
  }
}
