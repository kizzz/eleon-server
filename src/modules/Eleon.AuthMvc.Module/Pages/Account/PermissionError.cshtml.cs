using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace VPortal.Pages.Account
{
  public class PermissionErrorModel : AbpPageModel
  {
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ApplicationIdentifier { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string RequiredPolicy { get; set; }

    public void OnGet()
    {
      ReturnUrl = Request.Query["returnUrl"];
      ApplicationIdentifier = Request.Query["applicationIdentifier"];
      RequiredPolicy = Request.Query["requiredPolicy"];
    }

    public async Task<IActionResult> OnPostAsync()
    {
      var safeUrl = await GetRedirectUrlAsync(ReturnUrl);
      return Redirect(safeUrl);
    }
  }
}
