using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.ExternalLink.Module.DomainServices;
using VPortal.ExternalLink.Module.Managers;
using VPortal.ExternalLink.Module.ValueObjects;

namespace VPortal.ExternalLink.Module.FileExternalLink
{
  public class ExternalLinkAppService : ModuleAppService, IExternalLinkAppService
  {
    private readonly IVportalLogger<ExternalLinkAppService> logger;
    private readonly ExternalLinkOtpManager fileOtpManager;
    private readonly ExternalLinkDomainService domainService;

    public ExternalLinkAppService(IVportalLogger<ExternalLinkAppService> logger,
        ExternalLinkOtpManager fileOtpManager,
        ExternalLinkDomainService domainService)
    {
      this.logger = logger;
      this.fileOtpManager = fileOtpManager;
      this.domainService = domainService;
    }
    public async Task<ExternalLinkLoginInfoDto> GetLoginInfoAsync(string code)
    {

      ExternalLinkLoginInfoDto result = null;
      try
      {
        var link = await domainService.GetPublicParams(code);

        result = ObjectMapper.Map<ExternalLinkLoginInfoValueObject, ExternalLinkLoginInfoDto>(link.Value);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> DirectLoginAsync(string code, string password)
    {

      string result = null;
      try
      {
        ResultValueObject<string> resultValueObject = await domainService.GetPrivateParams(code, password);

        result = resultValueObject.Value;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<string> GetOtp(string linkCode)
    {

      string result = null;
      try
      {

        var link = await domainService.GetLinkAsync(linkCode);

        await domainService.RegisterLoginAttempt(link);

        if (!await domainService.CheckLoginAttempt(link))
        {
          return result;
        }

        var response = await fileOtpManager.SendOtpGenerationMessage(link.ExternalLinkCode, link.LoginType, link.LoginKey);
        result = response.Message;
        if (response.Success)
        {
          await domainService.SuccesfulLogin(link);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<string> LoginWithOtp(string linkCode, string otp)
    {
      string result = null;
      try
      {
        var link = await domainService.GetLinkAsync(linkCode);

        var validate = await fileOtpManager.SendOtpValidationMessage(linkCode, otp);

        if (validate.Valid)
        {
          result = link.PrivateParams;
          return result;
        }

        throw new UserFriendlyException(validate.ErrorMessage);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }
  }
}
