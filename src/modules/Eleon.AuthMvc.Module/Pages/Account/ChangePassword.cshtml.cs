using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Localization;

namespace VPortal.Pages.Account
{
  public class ChangePassword : AbpPageModel
  {
    private readonly PasswordChangeDomainService passwordChangeDomainService;
    private readonly IStringLocalizer<IdentityResource> stringLocalizer;

    [BindProperty]
    public ChangePasswordRequest InputData { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    public ChangePassword(
        PasswordChangeDomainService passwordChangeDomainService,
        IStringLocalizer<IdentityResource> stringLocalizer)
    {
      this.passwordChangeDomainService = passwordChangeDomainService;
      this.stringLocalizer = stringLocalizer;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      if (!CurrentUser.IsAuthenticated)
      {
        return RedirectToPage("Login");
      }

      ReturnUrl = Request.Query["returnUrl"];
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!CurrentUser.IsAuthenticated)
      {
        return RedirectToPage("Login");
      }

      if (!Validate())
      {
        return Page();
      }

      bool changed = await passwordChangeDomainService.ChangePassword(InputData.OldPassword, InputData.NewPassword);
      if (!changed)
      {
        Alerts.Danger(stringLocalizer["ChangePassword:InvalidOldPassword"]);
        return Page();
      }

      if (!string.IsNullOrWhiteSpace(ReturnUrl))
      {
        var safeUrl = await GetRedirectUrlAsync(ReturnUrl);
        return Redirect(safeUrl);
      }
      else
      {
        return Redirect("/ui");
      }
    }

    private bool Validate()
    {
      ValidateModel();

      bool passwordMatches = InputData.NewPassword == InputData.RepeatNewPassword;
      if (!passwordMatches)
      {
        Alerts.Danger(stringLocalizer["ChangePassword:RepeatNewPassword:Error"]);
        return false;
      }

      return true;
    }

    public class ChangePasswordRequest
    {
      [Required]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
      [DataType(DataType.Password)]
      [DisableAuditing]
      public string OldPassword { get; set; }

      [Required]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
      [DataType(DataType.Password)]
      [DisableAuditing]
      public string NewPassword { get; set; }

      [Required]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
      [DataType(DataType.Password)]
      [DisableAuditing]
      public string RepeatNewPassword { get; set; }
    }
  }
}
