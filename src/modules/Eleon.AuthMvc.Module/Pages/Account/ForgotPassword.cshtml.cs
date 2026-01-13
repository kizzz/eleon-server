using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Pages.Account
{
  public class ForgotPasswordModel : PageModel
  {
    private readonly PasswordRestoreDomainService passwordRestoreDomainService;

    [BindProperty]
    public RestoreRequestInformation InputData { get; set; }

    public ForgotPasswordModel(PasswordRestoreDomainService passwordRestoreDomainService)
    {
      this.passwordRestoreDomainService = passwordRestoreDomainService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!await Validate())
      {
        return Page();
      }

      await passwordRestoreDomainService.SendRestoreRequest(InputData.Email, InputData.Username);
      return RedirectToPage("RestoreLinkSent");
    }

    private async Task<bool> Validate()
    {
      if (string.IsNullOrWhiteSpace(InputData.Email))
      {
        return false;
      }

      if (string.IsNullOrWhiteSpace(InputData.Username))
      {
        return false;
      }

      return true;
    }

    public class RestoreRequestInformation
    {
      [Required]
      [DataType(DataType.EmailAddress)]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
      public string Email { get; set; }

      [Required]
      [DataType(DataType.Text)]
      [MinLength(4)]
      public string Username { get; set; }
    }
  }
}
