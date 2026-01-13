using Common.Module.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Themes.Basic.Components.Toolbar.UserMenu;
//using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Toolbar.UserMenu;
using Volo.Abp.DependencyInjection;
using Volo.Abp.UI.Navigation;

namespace VPortal.Overrides.Account
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(UserMenuViewComponent))]
  public class VportalUserMenuViewComponent : UserMenuViewComponent
  {
    public VportalUserMenuViewComponent(IMenuManager menuManager) : base(menuManager)
    {
    }

    public override async Task<IViewComponentResult> InvokeAsync()
    {
      bool? isDriverClient = HttpContext.User?.HasClaim(x => x.Type == OptionalUserClaims.ClientType && x.Value == "driver_client");
      if (isDriverClient == true)
      {
        return Content(string.Empty);
      }

      return await base.InvokeAsync();
    }
  }
}
