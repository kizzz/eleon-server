using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace VPortal.Policies
{
  public class RemoveAbpControllersConventions : IControllerModelConvention
  {
    public void Apply(ControllerModel controller)
    {
      if (controller.ControllerType.Namespace != null && controller.ControllerType.Namespace.StartsWith("Volo."))
      {
        controller.ApiExplorer.IsVisible = false;
        controller.RouteValues.Clear();
        controller.Actions.Clear();
        controller.Selectors.Clear();
        //controller.Selectors.Clear(); // Disables routing
      }
    }
  }
}
