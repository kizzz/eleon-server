using Common.Module.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Localization;

namespace VPortal.Pages.Account
{
  public class RestorePasswordModel : AbpPageModel
  {
    private readonly IStringLocalizer<IdentityResource> stringLocalizer;
    private readonly PasswordRestoreDomainService passwordRestoreDomainService;
    private readonly RegistrationDomainService registrationDomainService;
    private readonly IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource> customIdentityStringLocalizer;

    [BindProperty]
    public NewPasswordInput InputData { get; set; }

    [BindProperty(SupportsGet = true)]
    [HiddenInput]
    public string Code { get; set; }

    [BindProperty]
    public PasswordSettingsModel PasswordSettings { get; set; }

    public RestorePasswordModel(
        IStringLocalizer<IdentityResource> stringLocalizer,
        PasswordRestoreDomainService passwordRestoreDomainService,
        RegistrationDomainService registrationDomainService,
        IStringLocalizer<IdentityResource> customIdentityStringLocalizer)
    {
      this.stringLocalizer = stringLocalizer;
      this.passwordRestoreDomainService = passwordRestoreDomainService;
      this.registrationDomainService = registrationDomainService;
      LoadSettings().GetAwaiter().GetResult();
      this.customIdentityStringLocalizer = customIdentityStringLocalizer;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      string code = Request.Query["code"];
      bool valid = await passwordRestoreDomainService.ValidateRestoreCode(code);
      if (!valid)
      {
        return RedirectToPage("InvalidLink");
      }

      Code = code;
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!await Validate())
      {
        return Page();
      }

      bool changed = await passwordRestoreDomainService.ChangePassword(Code, InputData.Password);
      if (!changed)
      {
        return RedirectToPage("InvalidLink");
      }

      return RedirectToPage("PasswordRestored");
    }

    private async Task<bool> Validate()
    {
      await LoadSettings();

      string password = InputData.Password;

      List<string> errors = new List<string>();

      if (string.IsNullOrEmpty(password))
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordEmpty:Error"]);
      }

      if (!string.IsNullOrEmpty(password) && password.Length < PasswordSettings.PasswordRequiredLength)
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequiredLength:Error", PasswordSettings.PasswordRequiredLength]);
      }

      if (PasswordSettings.PasswordRequireDigit && !password.Any(char.IsDigit))
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireDigit:Error"]);
      }

      if (PasswordSettings.PasswordRequireLowercase && !password.Any(char.IsLower))
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireLowercase:Error"]);
      }

      if (PasswordSettings.PasswordRequireUppercase && !password.Any(char.IsUpper))
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireUppercase:Error"]);
      }

      if (PasswordSettings.PasswordRequireNonAlphanumeric && !password.Any(ch => !char.IsLetterOrDigit(ch)))
      {
        errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireNonAlphanumeric:Error"]);
      }

      if (PasswordSettings.PasswordRequiredUniqueChars > 0)
      {
        int uniqueCharCount = password.Distinct().Count();

        if (uniqueCharCount < PasswordSettings.PasswordRequiredUniqueChars)
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequiredUniqueChars:Error", PasswordSettings.PasswordRequiredUniqueChars]);
        }
      }

      if (errors.Count > 0)
      {
        errors.ForEach(x =>
        {
          Alerts.Danger(x);
        });
        return false;
      }

      bool passwordMatches = password == InputData.RepeatPassword;
      if (!passwordMatches)
      {
        Alerts.Danger(customIdentityStringLocalizer["RestorePassword:RepeatPassword:Error"]);
        return false;
      }

      return true;
    }

    private async Task LoadSettings()
    {
      var settings = await registrationDomainService.GetIdentitySettingsForRegistration();
      if (settings != null)
      {
        PasswordSettings = new PasswordSettingsModel();
        PasswordSettings.PasswordRequireUppercase = settings.PasswordRequireUppercase;
        PasswordSettings.PasswordRequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric;
        PasswordSettings.PasswordRequireLowercase = settings.PasswordRequireLowercase;
        PasswordSettings.PasswordRequireDigit = settings.PasswordRequireDigit;
        PasswordSettings.PasswordRequiredLength = settings.PasswordRequiredLength;
        PasswordSettings.PasswordRequiredUniqueChars = settings.PasswordRequiredUniqueChars;

        var passwordRules = "Use";
        if (PasswordSettings.PasswordRequiredLength > 0)
        {
          passwordRules += $" {PasswordSettings.PasswordRequiredLength} or more characters";
        }
        if (PasswordSettings.PasswordRequireNonAlphanumeric || PasswordSettings.PasswordRequireLowercase || PasswordSettings.PasswordRequireDigit || PasswordSettings.PasswordRequireUppercase)
        {
          passwordRules += " with a mix of";
          passwordRules += PasswordSettings.PasswordRequireLowercase ? " lowercase letters," : "";
          passwordRules += PasswordSettings.PasswordRequireUppercase ? " uppercase letters," : "";
          passwordRules += PasswordSettings.PasswordRequireDigit ? " numbers," : "";
          passwordRules += PasswordSettings.PasswordRequireNonAlphanumeric ? " symbols," : "";

          passwordRules = passwordRules.TrimEnd(',', ' ');
        }
        if (PasswordSettings.PasswordRequiredUniqueChars > 0)
        {
          passwordRules += $" and {PasswordSettings.PasswordRequiredUniqueChars} unique characters";
        }

        PasswordSettings.PasswordRules = passwordRules + ".";
      }
    }


    public class PasswordSettingsModel()
    {
      public int PasswordRequiredLength { get; set; }

      public int PasswordRequiredUniqueChars { get; set; }

      public bool PasswordRequireNonAlphanumeric { get; set; }

      public bool PasswordRequireLowercase { get; set; }

      public bool PasswordRequireUppercase { get; set; }

      public bool PasswordRequireDigit { get; set; }

      [BindProperty]
      public string PasswordRules { get; set; }
    }

    public class NewPasswordInput
    {
      [Required]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
      [DataType(DataType.Password)]
      [DisableAuditing]
      public string Password { get; set; }

      [Required]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
      [DataType(DataType.Password)]
      [DisableAuditing]
      public string RepeatPassword { get; set; }
    }
  }
}
