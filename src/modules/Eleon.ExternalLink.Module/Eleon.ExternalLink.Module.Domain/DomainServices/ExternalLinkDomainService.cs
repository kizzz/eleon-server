using Common.Module.Constants;
using Common.Module.Helpers;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.ExternalLink.Module.Entities;
using VPortal.ExternalLink.Module.Repositories;
using VPortal.ExternalLink.Module.ValueObjects;

namespace VPortal.ExternalLink.Module.DomainServices
{
  public class ExternalLinkDomainService : DomainService
  {
    private readonly IVportalLogger<ExternalLinkDomainService> logger;
    private readonly IObjectMapper<ExternalLinkDomainModule> objectMapper;
    private readonly IExternalLinkRepository repository;

    public ExternalLinkDomainService(
        IVportalLogger<ExternalLinkDomainService> logger,
        IObjectMapper<ExternalLinkDomainModule> objectMapper,
        IExternalLinkRepository repository)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.repository = repository;
    }

    private async Task FillLabel(ExternalLinkEntity link)
    {
      try
      {
        if (link.LoginType == ExternalLinkLoginType.Email)
        {
          string pattern = @"(?<=[\w]{1})[\w\-._\+%]*(?=[\w]{1}@)";
          link.LoginKeyLabel = Regex.Replace(link.LoginKey, pattern, m => new string('*', m.Length));
        }

        if (link.LoginType == ExternalLinkLoginType.Sms)
        {
          string pattern = @"\d(?!\d{0,3}$)";
          link.LoginKeyLabel = Regex.Replace(link.LoginKey, pattern, m => new string('*', m.Length));
        }

        if (link.LoginType == ExternalLinkLoginType.Password)
        {
          link.LoginKeyLabel = link.LoginKey;
        }
      }
      catch (Exception)
      {
        link.LoginKeyLabel = "Not Found";
      }
    }

    public async Task<bool> CheckLoginAttempt(ExternalLinkEntity link)
    {
      bool result = false;
      try
      {
        if (link.Status == Common.Module.Constants.LinkShareStatus.Canceled)
        {
          return false;
        }

        if (DateTime.Now > link.ExpirationDateTime)
        {
          return false;
        }

        if (link.Status == Common.Module.Constants.LinkShareStatus.Suspended &&
            link.LastLoginAttemptDate > DateTime.Now.Subtract(new TimeSpan(0, 15, 0)))
        {
          return false;
        }

        result = true;
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

    public async Task<bool> SuccesfulLogin(ExternalLinkEntity link)
    {
      bool result = false;
      try
      {
        link.LoginAttempts = 0;
        if (link.IsOneTimeLink)
        {
          link.Status = LinkShareStatus.Canceled;
        }

        await repository.UpdateAsync(link, true);

        result = true;
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
    public async Task<bool> RegisterLoginAttempt(ExternalLinkEntity link)
    {
      bool result = false;
      try
      {

        link.LoginAttempts++;
        link.LastLoginAttemptDate = DateTime.Now;

        if (link.LoginAttempts == 5 && link.LastLoginAttemptDate > DateTime.Now.Subtract(new TimeSpan(0, 15, 0)))
        {
          link.Status = Common.Module.Constants.LinkShareStatus.Suspended;
          link.LoginAttempts = 0;
        }

        await repository.UpdateAsync(link, true);
        result = true;
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

    public async Task<ExternalLinkEntity> GetLinkAsync(string code)
    {
      ExternalLinkEntity result = null;
      try
      {
        result = await repository.GetAsync(code);
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

    public async Task<ExternalLinkEntity> Create(ExternalLinkEntity externalLinkEntity)
    {

      ExternalLinkEntity result = null;
      try
      {
        result = new ExternalLinkEntity(GuidGenerator.Create())
        {
          ExpirationDateTime = externalLinkEntity.ExpirationDateTime,
          ExternalLinkCode = GuidGenerator.Create().ToString(),
          ExternalLinkUrl = externalLinkEntity.ExternalLinkUrl,
          LoginAttempts = 0,
          LoginKey = externalLinkEntity.LoginKey,
          LoginType = externalLinkEntity.LoginType,
          DocumentType = externalLinkEntity.DocumentType,
          PrivateParams = externalLinkEntity.PrivateParams,
          PublicParams = externalLinkEntity.PublicParams,
          Status = externalLinkEntity.Status,
        };

        await repository.InsertAsync(result, true);
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

    public async Task<ExternalLinkEntity> Update(ExternalLinkEntity externalLinkEntity)
    {
      ExternalLinkEntity result = null;
      try
      {
        var oldEntity = await repository.GetAsync(externalLinkEntity.Id);


        oldEntity.ExpirationDateTime = externalLinkEntity.ExpirationDateTime;
        oldEntity.ExternalLinkUrl = externalLinkEntity.ExternalLinkUrl;
        oldEntity.LoginType = externalLinkEntity.LoginType;
        oldEntity.DocumentType = externalLinkEntity.DocumentType;
        oldEntity.PrivateParams = externalLinkEntity.PrivateParams;
        oldEntity.PublicParams = externalLinkEntity.PublicParams;
        oldEntity.Status = externalLinkEntity.Status;

        result = await repository.UpdateAsync(oldEntity, true);
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

    public async Task Delete(string externalLinkCode)
    {
      try
      {
        var link = await GetLinkAsync(externalLinkCode);

        await repository.DeleteAsync(link, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<ResultValueObject<string>> GetPrivateParams(string linkCode, string password)
    {

      ResultValueObject<string> result = new ResultValueObject<string>();
      try
      {
        var link = await GetLinkAsync(linkCode);

        if (link == null)
        {
          return ResultHelper.Fail<string>("Link is missing");
        }


        if (!await CheckLoginAttempt(link))
        {
          return ResultHelper.Fail<string>("Wait for next login.");
        }

        await RegisterLoginAttempt(link);

        bool isValidPass = link.LoginType == ExternalLinkLoginType.Password && !password.IsNullOrEmpty() && link.LoginKey == password;
        bool isNoLogin = link.LoginType == ExternalLinkLoginType.None;
        if (isNoLogin || isValidPass)
        {
          await SuccesfulLogin(link);
          return ResultHelper.Ok(link.PrivateParams);
        }
      }
      catch (Exception e)
      {
        return ResultHelper.Fail<string>(e.Message);
      }
      finally
      {
      }

      return ResultHelper.Fail<string>("Incorrect link information.");
    }
    public async Task<ResultValueObject<ExternalLinkLoginInfoValueObject>> GetPublicParams(string linkCode)
    {

      ResultValueObject<ExternalLinkLoginInfoValueObject> result = new ResultValueObject<ExternalLinkLoginInfoValueObject>();
      try
      {
        var link = await GetLinkAsync(linkCode);

        if (link == null)
        {
          return ResultHelper.Fail<ExternalLinkLoginInfoValueObject>("Link is missing");
        }

        if (link.ExpirationDateTime < DateTime.Now)
        {
          return ResultHelper.Fail<ExternalLinkLoginInfoValueObject>("Expiration date is missing");
        }

        if (link.Status != LinkShareStatus.Active)
        {
          return ResultHelper.Fail<ExternalLinkLoginInfoValueObject>("Link status is inactive.");
        }

        var loginInfo = objectMapper.Map<ExternalLinkEntity, ExternalLinkLoginInfoValueObject>(link);
        return ResultHelper.Ok(loginInfo);
      }
      catch (Exception e)
      {
        return ResultHelper.Fail<ExternalLinkLoginInfoValueObject>(e.Message);
      }
      finally
      {
      }

    }


    public async Task<bool> CheckLastLinkActive(string privateParams, string docType)
    {
      bool result = false;
      try
      {
        var externalLinks = await repository.GetByPrivateParamsAndDocTypeAsync(privateParams, docType);
        if (externalLinks != null && externalLinks.Count > 0)
        {
          externalLinks = externalLinks.OrderByDescending(x => x.ExpirationDateTime).ToList();
          var link = externalLinks.First();

          if ((link.ExpirationDateTime < DateTime.Now) || (link.Status != LinkShareStatus.Active))
          {
            result = true;
          }
        }
        else
        {
          result = true;
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
