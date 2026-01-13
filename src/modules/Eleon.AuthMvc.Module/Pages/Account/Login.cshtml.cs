using Authorization.Module.TenantHostname;
using Common.Module.Constants;
using Common.Module.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Account.Settings;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using VPortal.Identity.Module.CustomCredentials;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Pages.Account
{
  [ExposeServices(typeof(LoginModel), typeof(VportalLoginModel))]
  public class VportalLoginModel : LoginModel, IScopedDependency
  {
    private readonly IConfiguration configuration;
    private readonly LocalSignInDomainService localSignInDomainService;
    private readonly SignInDomainService signInDomainService;
    private readonly IStringLocalizer<IdentityResource> stringLocalizer;
    private readonly TenantUrlResolver tenantUrlResolver;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly RegistrationDomainService registrationDomainService;
    private readonly IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource> customIdentityStringLocalizer;
    private readonly PasswordRestoreDomainService passwordRestoreDomainService;

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool IsTwoFactorSecondStep { get; set; }

    [BindProperty]
    public string OtpSuccessMessage { get; set; }

    [BindProperty]
    public VehicleDriverInputModel VehicleDriverInput { get; set; }

    [BindProperty]
    public OtpInputModel OtpInput { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool InsidePopup { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool? InsideIframe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool IsRegistrationProcess { get; set; }

    [BindProperty]
    public RegistrationUserDataModel UserRegistrationData { get; set; } = new RegistrationUserDataModel();

    [BindProperty]
    public SettingsModel Settings { get; set; } = new SettingsModel();

    [BindProperty]
    public RestoreRequestInformationModel RestorePasswordInputData { get; set; } = new RestoreRequestInformationModel();

    public VportalLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IOptions<IdentityOptions> identityOptions,
        IConfiguration configuration,
        LocalSignInDomainService localSignInDomainService,
        SignInDomainService signInDomainService,
        IStringLocalizer<IdentityResource> stringLocalizer,
        IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource> customIdentityStringLocalizer,
        TenantUrlResolver tenantUrlResolver,
        IHttpContextAccessor httpContextAccessor,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        RegistrationDomainService registrationDomainService,
        PasswordRestoreDomainService passwordRestoreDomainService,
        IWebHostEnvironment webHostEnvironment)
        : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
    {
      this.configuration = configuration;
      this.localSignInDomainService = localSignInDomainService;
      this.signInDomainService = signInDomainService;
      this.stringLocalizer = stringLocalizer;
      this.customIdentityStringLocalizer = customIdentityStringLocalizer;
      this.tenantUrlResolver = tenantUrlResolver;
      this.httpContextAccessor = httpContextAccessor;
      this.registrationDomainService = registrationDomainService;
      LoadSettings().GetAwaiter().GetResult();
      this.passwordRestoreDomainService = passwordRestoreDomainService;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
      InsideIframe = null;
      return Page();
    }

    public async Task<IActionResult> OnGetDefault()
    {
      VehicleDriverInput = new VehicleDriverInputModel();
      OtpInput = new OtpInputModel();
      LoginInput = new LoginInputModel();
      InsideIframe = false;
      IsRegistrationProcess = TempData["IsRegistration"] as string == "True";

      ExternalProviders = await GetExternalProviders();

      EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

      if (InsideIframe != false)
      {
        return Page();
      }

      if (await signInDomainService.HasExternalLogin())
      {
        TempData["IsRegistration"] = false.ToString();
        if (HttpContext.Request.Query.ContainsKey("resendOtp"))
        {
          var result = await signInDomainService.SignInWithOtpAsync(isRegistrationProcess: IsRegistrationProcess);

          if (!result.SignInResult.Succeeded)
          {
            SetResendAllowed();
            IsTwoFactorSecondStep = true;
          }

          return await HandleSignInResult(result);
        }
      }

      bool hasAzureScheme = ExternalProviders.Any(x => x.AuthenticationScheme == nameof(ExternalLoginProviderType.AzureEntra));
      if (IsExternalLoginOnly || hasAzureScheme)
      {
        return await OnPostExternalLogin(ExternalProviders.First().AuthenticationScheme);
      }

      return Page();
    }

    private void SetResendAllowed()
    {
      var time = configuration.GetSection("OTP:ResendTime").Get<int>();
      if (time <= 0)
      {
        time = 30;
      }
      ViewData["ResendAllowed"] = DateTimeOffset.Now.AddSeconds(time);
    }

    public override async Task<IActionResult> OnPostExternalLogin(string provider)
    {
      var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash, InsidePopup });
      var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      properties.Items["scheme"] = provider;

      return await Task.FromResult(Challenge(properties, provider));
    }

    public async Task<IActionResult> OnGetPopup()
    {
      InsidePopup = true;
      return await OnGetDefault();
    }

    public async Task<IActionResult> OnGetCancel()
    {
      IsTwoFactorSecondStep = false;
      TempData["IsRegistration"] = false.ToString();
      IsRegistrationProcess = false;
      await signInDomainService.ClearExternalLogin();
      return Page();
    }

    public async Task<IActionResult> OnGetIframe()
    {
      InsideIframe = true;
      return Page();
    }

    public async Task<IActionResult> OnGetAfterLoggedInPopup()
    {
      return RedirectSafelyAsync(ReturnUrl).GetAwaiter().GetResult();
    }

    public async Task<IActionResult> OnGetReturnWithoutLogin()
    {
      var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(ReturnUrl);
      return Redirect(query["redirect_uri"].First());
    }

    public override async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
    {
      if (remoteError != null)
      {
        Logger.LogWarning($"External login callback error: {remoteError}");
        return RedirectToPage("./Login");
      }

      var result = await signInDomainService.SignInWithOtpAsync();
      SetResendAllowed();
      return await HandleSignInResult(result);
    }

    // The following method is based on the ABP 9.0.2 source code (Volo.Abp.Account.Web\Pages\Account\login.cshtml.cs)
    public override async Task<IActionResult> OnPostAsync(string action)
    {
      CustomSignInResult result = null;
      try
      {
        IsRegistrationProcess = TempData["IsRegistration"] as string == "True";

        await LoadSettings();

        await CheckLocalLoginAsync();

        ExternalProviders = await GetExternalProviders();

        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        await ReplaceEmailToUsernameOfInputIfNeeds();

        await IdentityOptions.SetAsync();

        if (!await Validate())
        {
          return Page();
        }

        result = await LocalSignIn();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "An exception occurred while processing sign in");
        result = new CustomSignInResult(Microsoft.AspNetCore.Identity.SignInResult.Failed);
      }

      return await HandleSignInResult(result);
    }

    public string GetHandlerUrl(string handler)
    {
      var currentUrl = HttpContext.Request.GetEncodedUrl();
      var uriBuilder = new UriBuilder(currentUrl);
      var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uriBuilder.Query);
      query["handler"] = handler;
      uriBuilder.Query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("", query.ToDictionary(x => x.Key, x => x.Value.First()));
      return uriBuilder.Uri.ToString();
    }

    private async Task<IActionResult> HandleSignInResult(CustomSignInResult result)
    {
      IsRegistrationProcess = TempData["IsRegistration"] as string == "True";
      if (!result.RequireReload && !result.RequireSecureApi && result.SignInResult.RequiresTwoFactor)
      {
        IsTwoFactorSecondStep = true;
      }

      if (IsRegistrationProcess)
      {
        await LoadSettings();
        IsTwoFactorSecondStep = Settings.EnableTwoAuth;
      }

      if (result.SignInResult.RequiresTwoFactor)
      {
        ViewData["prevCode"] = OtpInput?.OtpValue;
      }

      await WriteMessages(result);

      return await GetPostLoginAction(result);
    }

    private async Task<IActionResult> GetPostLoginAction(CustomSignInResult result)
    {
      await LoadSettings();

      if (result.RequireSecureApi)
      {
        string secureBaseUrl = await tenantUrlResolver.GetBaseTenantUrl(HttpContext, CurrentTenant.Id, security: true);
        string secureUrl = $"{secureBaseUrl}/Account/Login";
        string returnUrl = Request.Query["ReturnUrl"];
        if (returnUrl.NonEmpty())
        {
          string encodedReturnUrl = Uri.EscapeDataString(returnUrl);
          secureUrl += $"?ReturnUrl={encodedReturnUrl}";
        }

        return Redirect(secureUrl);
      }

      if (result.RequireReload)
      {
        return RedirectToPage("/Account/Login", new { ReturnUrl = Request.Query["ReturnUrl"] });
      }

      if (!IsRegistrationProcess && result.SignInResult.RequiresTwoFactor)
      {
        IsTwoFactorSecondStep = true;
      }

      if (IsRegistrationProcess)
      {
        IsTwoFactorSecondStep = Settings.EnableTwoAuth;
        if (!IsTwoFactorSecondStep)
        {
          return RedirectSafelyAsync(ReturnUrl).GetAwaiter().GetResult();
        }
      }

      if (!result.SignInResult.Succeeded)
      {
        return Page();
      }

      if (InsidePopup)
      {
        return RedirectToPage("PostLoginInPopup");
      }
      else
      {
        return RedirectSafelyAsync(ReturnUrl ?? "/").GetAwaiter().GetResult();
      }
    }

    private async Task<CustomSignInResult> LocalSignIn()
    {
      if (IsTwoFactorSecondStep)
      {
        return await signInDomainService.SignInWithOtpAsync(OtpInput.OtpValue, isRegistrationProcess: IsRegistrationProcess);
      }

      var result = await localSignInDomainService.SignInAsync(
                  LoginInput.UserNameOrEmailAddress,
                  LoginInput.Password);
      SetResendAllowed();
      return result;
    }

    private async Task WriteMessages(CustomSignInResult result)
    {
      if (result.SuccessMessage.NonEmpty() && IsTwoFactorSecondStep)
      {
        OtpSuccessMessage = result.SuccessMessage;
      }
      else if (result.SuccessMessage.NonEmpty() && !IsTwoFactorSecondStep)
      {
        Alerts.Success(result.SuccessMessage);
      }
      else if (result.ErrorMessage.NonEmpty())
      {
        Alerts.Danger(result.ErrorMessage);
      }
      else if (!result.SignInResult.Succeeded && !IsTwoFactorSecondStep)
      {
        Alerts.Danger(L["InvalidUserNameOrPassword"]);
      }

      //await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
      //{
      //    Identity = IdentitySecurityLogIdentityConsts.Identity,
      //    Action = result.SignInResult.ToIdentitySecurityLogAction(),
      //    //UserName = LoginInput.UserNameOrEmailAddress
      //});

      if (result.SignInResult.IsLockedOut)
      {
        Alerts.Warning(L["UserLockedOutMessage"]);
      }

      if (result.SignInResult.IsNotAllowed)
      {
        Alerts.Warning(L["LoginIsNotAllowed"]);
      }
    }

    private async Task<bool> Validate()
    {
      IsRegistrationProcess = TempData["IsRegistration"] as string == "True";

      if (IsTwoFactorSecondStep)
      {
        if (OtpInput.OtpValue.IsNullOrWhiteSpace())
        {
          return false;
        }
      }

      if (!IsTwoFactorSecondStep)
      {
        if (LoginInput.UserNameOrEmailAddress.IsNullOrWhiteSpace())
        {
          return false;
        }

        if (Settings.EnablePassword && LoginInput.Password.IsNullOrWhiteSpace())
        {
          return false;
        }
      }

      return true;
    }

    private async Task LoadSettings()
    {
      var settings = await registrationDomainService.GetIdentitySettingsForRegistration();
      if (settings != null)
      {
        Settings.AllowChangeEmail = settings.AllowChangeEmail;
        Settings.AllowChangeUserName = settings.AllowChangeUserName;
        Settings.EnablePassword = settings.EnablePassword;
        Settings.EnableSelfRegistration = settings.EnableSelfRegistration;
        Settings.EnableTwoAuth = settings.EnableTwoAuth;

        Settings.PasswordRequireUppercase = settings.PasswordRequireUppercase;
        Settings.PasswordRequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric;
        Settings.PasswordRequireLowercase = settings.PasswordRequireLowercase;
        Settings.PasswordRequireDigit = settings.PasswordRequireDigit;
        Settings.PasswordRequiredLength = settings.PasswordRequiredLength;
        Settings.PasswordRequiredUniqueChars = settings.PasswordRequiredUniqueChars;

        var passwordRules = "Use";
        if (Settings.PasswordRequiredLength > 0)
        {
          passwordRules += $" {Settings.PasswordRequiredLength} or more characters";
        }
        if (Settings.PasswordRequireNonAlphanumeric || Settings.PasswordRequireLowercase || Settings.PasswordRequireDigit || Settings.PasswordRequireUppercase)
        {
          passwordRules += " with a mix of";
          passwordRules += Settings.PasswordRequireLowercase ? " lowercase letters," : "";
          passwordRules += Settings.PasswordRequireUppercase ? " uppercase letters," : "";
          passwordRules += Settings.PasswordRequireDigit ? " numbers," : "";
          passwordRules += Settings.PasswordRequireNonAlphanumeric ? " symbols," : "";

          passwordRules = passwordRules.TrimEnd(',', ' ');
        }
        if (Settings.PasswordRequiredUniqueChars > 0)
        {
          passwordRules += $" and {Settings.PasswordRequiredUniqueChars} unique characters";
        }

        Settings.PasswordRules = passwordRules + ".";
      }
    }

    public class VehicleDriverInputModel
    {
      [Required]
      public string VehicleNumber { get; set; }

      [DataType(DataType.PhoneNumber)]
      [DisableAuditing]
      [Required]
      public string DriverMobile { get; set; }
    }

    public class OtpInputModel
    {
      [DataType(DataType.Text)]
      [MaxLength(6)]
      [MinLength(6)]
      [Required]
      public string OtpValue { get; set; }
    }

    #region Registration Process

    public async Task<IActionResult> OnGetRegistration()
    {
      IsRegistrationProcess = true;
      return new JsonResult(new { success = true, IsRegistration = true, Settings.PasswordRules });
    }

    public async Task<IActionResult> OnPostRegistration([FromBody] RegistrationUserDataModel model)
    {
      await LoadSettings();
      UserRegistrationData = model;
      IsRegistrationProcess = true;
      TempData["IsRegistration"] = IsRegistrationProcess.ToString();
      var errorMessage = ValidateSelfRegistration();

      if (!errorMessage.IsNullOrEmpty())
      {
        return new JsonResult(new { success = false, errorMessage });
      }

      CustomSignInResult result = await registrationDomainService.CreateUser(UserRegistrationData.Username,
                                                                              UserRegistrationData.Name,
                                                                              UserRegistrationData.Surname,
                                                                              UserRegistrationData.Email,
                                                                              UserRegistrationData.PhoneNumber,
                                                                              UserRegistrationData.Password);
      if (result.ErrorMessage.IsNullOrWhiteSpace())
      {
        return new JsonResult(new { success = true, EnablePassword = Settings.EnablePassword });
      }
      else
      {
        Alerts.Danger(result.ErrorMessage);
        return new JsonResult(new { success = false, result.ErrorMessage });
      }
    }

    private string ValidateSelfRegistration()
    {
      UserRegistrationData.Username = CleanUsername(UserRegistrationData.Username);

      if (UserRegistrationData.Name.IsNullOrEmpty())
      {
        return customIdentityStringLocalizer["Registration:NameEmpty:Error"];
      }

      if (UserRegistrationData.Surname.IsNullOrEmpty())
      {
        return customIdentityStringLocalizer["Registration:SurnameEmpty:Error"];
      }

      if (Settings.AllowChangeEmail && UserRegistrationData.Email.IsNullOrEmpty())
      {
        return customIdentityStringLocalizer["Registration:EmailEmpty:Error"];
      }

      if (Settings.AllowChangeEmail && !UserRegistrationData.Email.IsNullOrEmpty() && !ValidateEmail(UserRegistrationData.Email))
      {
        return customIdentityStringLocalizer["Registration:Email:Invalid:Error"];
      }

      if (!UserRegistrationData.Email.IsNullOrEmpty() && !Settings.AllowChangeUserName)
      {
        UserRegistrationData.Username = UserRegistrationData.Email?.Split('@')[0];
      }

      if (Settings.AllowChangeUserName && UserRegistrationData.Username.IsNullOrEmpty())
      {
        return customIdentityStringLocalizer["Registration:UsernameEmpty:Error"];
      }

      if (!UserRegistrationData.PhoneNumber.IsNullOrEmpty() && UserRegistrationData.PhoneNumber.Length > 16)
      {
        return customIdentityStringLocalizer["Registration:PhoneNumberLength:Error"];
      }

      if (Settings.EnablePassword)
      {
        string password = UserRegistrationData.Password;

        List<string> errors = new List<string>();

        if (string.IsNullOrEmpty(password))
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordEmpty:Error"]);
        }

        if (!string.IsNullOrEmpty(password) && password.Length < Settings.PasswordRequiredLength)
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequiredLength:Error", Settings.PasswordRequiredLength]);
        }

        if (Settings.PasswordRequireDigit && !password.Any(char.IsDigit))
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireDigit:Error"]);
        }

        if (Settings.PasswordRequireLowercase && !password.Any(char.IsLower))
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireLowercase:Error"]);
        }

        if (Settings.PasswordRequireUppercase && !password.Any(char.IsUpper))
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireUppercase:Error"]);
        }

        if (Settings.PasswordRequireNonAlphanumeric && !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
          errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequireNonAlphanumeric:Error"]);
        }

        if (Settings.PasswordRequiredUniqueChars > 0)
        {
          int uniqueCharCount = password.Distinct().Count();

          if (uniqueCharCount < Settings.PasswordRequiredUniqueChars)
          {
            errors.Add(customIdentityStringLocalizer["Registration:PasswordRules:PasswordRequiredUniqueChars:Error", Settings.PasswordRequiredUniqueChars]);
          }
        }

        if (errors.Count > 0)
        {
          return errors.JoinAsString("\r\n");
        }

        bool passwordMatches = password == UserRegistrationData.RepeatPassword;
        if (!passwordMatches)
        {
          return customIdentityStringLocalizer["Registration:RepeatPassword:Error"];
        }
      }

      return string.Empty;
    }

    private bool ValidateEmail(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        return false;

      string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

      return Regex.IsMatch(email, emailPattern);
    }

    private string CleanUsername(string username)
    {
      if (string.IsNullOrEmpty(username)) return null;

      username = username.Trim();
      var allowedUsernamePattern = @"[^a-zA-Z0-9._-]";
      return Regex.Replace(username, allowedUsernamePattern, string.Empty);
    }

    public class RegistrationUserDataModel
    {
      [Required]
      [DataType(DataType.Text)]
      public string Username { get; set; }

      [Required]
      [DataType(DataType.Text)]
      public string Name { get; set; }

      [Required]
      [DataType(DataType.Text)]
      public string Surname { get; set; }

      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
      [EmailAddress(ErrorMessage = "Invalid email address")]
      [DataType(DataType.EmailAddress)]
      public string Email { get; set; }

      [DataType(DataType.PhoneNumber)]
      [MaxLength(16, ErrorMessage = "Phone number length must not exceed 16 characters.")]
      public string PhoneNumber { get; set; }

      [DataType(DataType.Password)]
      public string Password { get; set; }

      [DataType(DataType.Password)]
      public string RepeatPassword { get; set; }
    }

    public class SettingsModel()
    {
      [BindProperty]
      public bool AllowChangeEmail { get; set; }

      [BindProperty]
      public bool AllowChangeUserName { get; set; }

      [BindProperty]
      public bool EnablePassword { get; set; }

      [BindProperty]
      public bool EnableTwoAuth { get; set; }

      [BindProperty]
      public bool EnableSelfRegistration { get; set; }

      [BindProperty]
      public int PasswordRequiredLength { get; set; }

      [BindProperty]
      public int PasswordRequiredUniqueChars { get; set; }

      [BindProperty]
      public bool PasswordRequireNonAlphanumeric { get; set; }

      [BindProperty]
      public bool PasswordRequireLowercase { get; set; }

      [BindProperty]
      public bool PasswordRequireUppercase { get; set; }

      [BindProperty]
      public bool PasswordRequireDigit { get; set; }

      public string PasswordRules { get; set; }
    }
    #endregion

    #region Forgot Password

    public async Task<IActionResult> OnPostRestorePassword([FromBody] RestoreRequestInformationModel model)
    {
      RestorePasswordInputData = model;
      var errorMsg = await ValidateRestoreData();
      if (!string.IsNullOrEmpty(errorMsg))
      {
        return new JsonResult(new { success = false, errorMessage = errorMsg });
      }

      var result = await passwordRestoreDomainService.SendRestoreRequest(RestorePasswordInputData.Email, RestorePasswordInputData.Username);
      if (result.IsNullOrEmpty())
      {
        return new JsonResult(new { success = true });
      }

      return new JsonResult(new { success = false, errorMessage = result });
    }

    private async Task<string> ValidateRestoreData()
    {
      if (RestorePasswordInputData.Email.IsNullOrWhiteSpace())
      {
        return customIdentityStringLocalizer["ForgotPassword:EmailEmpty:Error"];
      }

      bool isValidEmailFormat = Regex.IsMatch(RestorePasswordInputData.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
      if (!isValidEmailFormat)
      {
        return customIdentityStringLocalizer["ForgotPassword:EmailFormat:Error"];
      }


      if (RestorePasswordInputData.Username.IsNullOrWhiteSpace())
      {
        return customIdentityStringLocalizer["ForgotPassword:UsernameEmpty:Error"];
      }

      return null;
    }

    public class RestoreRequestInformationModel
    {
      [Required]
      [DataType(DataType.EmailAddress)]
      [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
      [EmailAddress(ErrorMessage = "Invalid email address")]
      public string Email { get; set; }

      [Required]
      [DataType(DataType.Text)]
      [MinLength(4)]
      public string Username { get; set; }
    }
    #endregion
  }
}
