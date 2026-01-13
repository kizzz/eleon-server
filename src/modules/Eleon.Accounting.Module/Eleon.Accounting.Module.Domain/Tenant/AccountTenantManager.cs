using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using EleonsoftModuleCollector.Commons.Module.Constants.BackgroundJobs;
using Logging.Module;
using Messaging.Module.Messages;
using ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Repositories;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace VPortal.Accounting.Module.Tenant
{

  public class AccountTenantManager : DomainService
  {
    private readonly IVportalLogger<AccountTenantManager> logger;
    private readonly IAccountRepository accountRepository;
    private readonly IObjectMapper objectMapper;
    private readonly XmlSerializerHelper xmlSerializerHelper;
    private readonly IDistributedEventBus eventBus;

    public AccountTenantManager(
        IVportalLogger<AccountTenantManager> logger,
        IDistributedEventBus eventBus,
        IAccountRepository accountRepository,
        IObjectMapper objectMapper,
        XmlSerializerHelper xmlSerializerHelper
      )
    {
      this.logger = logger;
      this.eventBus = eventBus;
      this.accountRepository = accountRepository;
      this.objectMapper = objectMapper;
      this.xmlSerializerHelper = xmlSerializerHelper;
    }

    public async Task<bool> RequestCreateTenantFromAccountJobExecution(Guid accountId)
    {
      bool result = false;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var accountToMap = await accountRepository.FindAsync(accountId);
          var mapped = objectMapper.Map<AccountEntity, AccountValueObject>(accountToMap);

          mapped.CreateDatabase = true;
          mapped.NewDatabaseName = accountToMap.AccountName;
          string xml = xmlSerializerHelper.SerializeToXml(mapped);
          var msg = new CreateBackgroundJobMsg
          {
            Id = GuidGenerator.Create(),
            ScheduleExecutionDateUtc = DateTime.UtcNow,
            TenantId = CurrentTenant.Id,
            StartExecutionParams = xml,
            IsRetryAllowed = true,
            Type = AccountingBackgroundJobTypes.CreateTenantFromAccount,
            StartExecutionExtraParams = accountId.ToString(),
            SourceType = BackgroundJobConstants.SourceType.SystemModule,
            SourceId = "Accounting"
          };

          await eventBus.PublishAsync(msg);
          result = true;
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<string> RequestResetAdminPasswordAccountTenantJobExecution(Guid accountId)
    {
      string result = string.Empty;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var accountToMap = await accountRepository.FindAsync(accountId);
          var mapped = objectMapper.Map<AccountEntity, AccountValueObject>(accountToMap);
          mapped.AdminPassword = GeneratePassword();
          string xml = xmlSerializerHelper.SerializeToXml(mapped);
          var msg = new CreateBackgroundJobMsg
          {
            Id = GuidGenerator.Create(),
            ScheduleExecutionDateUtc = DateTime.UtcNow,
            TenantId = CurrentTenant.Id,
            StartExecutionParams = xml,
            IsRetryAllowed = true,
            Type = AccountingBackgroundJobTypes.ResetAdminPasswordAccountTenant,
            StartExecutionExtraParams = accountId.ToString(),
            SourceType = BackgroundJobConstants.SourceType.SystemModule,
            SourceId = "Accounting"
          };
          await eventBus.PublishAsync(msg);
        }
      }
      catch (Exception e)
      {
        result = e.Message;
        logger.CaptureAndSuppress(e);
      }

      return result;
    }

    public async Task<string> RequestAccountTenantActionJobExecution(Guid accountId, string backgroundJobType)
    {
      string result = string.Empty;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var accountToMap = await accountRepository.FindAsync(accountId);
          var mapped = objectMapper.Map<AccountEntity, AccountValueObject>(accountToMap);
          string xml = xmlSerializerHelper.SerializeToXml(mapped);
          var jobId = GuidGenerator.Create();
          var msg = new CreateBackgroundJobMsg
          {
            Id = jobId,
            ScheduleExecutionDateUtc = DateTime.UtcNow,
            TenantId = CurrentTenant.Id,
            StartExecutionParams = xml,
            IsRetryAllowed = true,
            Type = backgroundJobType,
            StartExecutionExtraParams = accountId.ToString(),
            SourceType = BackgroundJobConstants.SourceType.SystemModule,
            SourceId = "Accounting"
          };

          await eventBus.PublishAsync(msg);
        }
      }
      catch (Exception e)
      {
        result = e.Message;
        logger.CaptureAndSuppress(e);
      }

      return result;
    }

    public async Task RequestCancelAccountTenant(AccountValueObject accountValueObject, Guid jobId)
    {
      try
      {
        using (CurrentTenant.Change(null))
        {
          var requestedMessage = new CancelAccountTenantMsg()
          {
            DocumentObjectType = "Account",
            AccountId = accountValueObject.Id,
            AccountTenantId = accountValueObject.AccountTenantId,
            DocumentId = accountValueObject.Id.ToString(),
          };

          var response = await eventBus.RequestAsync<AccountTenantActionMsg>(requestedMessage);
          if (!string.IsNullOrEmpty(response.ErrorMsg))
          {
            throw new Exception(response.ErrorMsg);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }

    }

    public async Task RequestActivateAccountTenant(AccountValueObject accountValueObject, Guid jobId)
    {
      Guid? result = Guid.Empty;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var requestedMessage = new ActivateAccountTenantMsg
          {
            AccountId = accountValueObject.Id,
            AccountTenantId = accountValueObject.AccountTenantId,
            DocumentId = accountValueObject.Id.ToString()
          };

          var response = await eventBus.RequestAsync<AccountTenantActionMsg>(requestedMessage);
          if (!string.IsNullOrEmpty(response.ErrorMsg))
          {
            throw new Exception(response.ErrorMsg);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }

    }

    public async Task RequestSuspendAccountTenant(AccountValueObject accountValueObject, Guid jobId)
    {
      try
      {
        using (CurrentTenant.Change(null))
        {
          var requestedMessage = new SuspendAccountTenantMsg
          {
            ObjectType = "Account",
            DocumentId = accountValueObject.Id.ToString(),
            AccountId = accountValueObject.Id,
            AccountTenantId = accountValueObject.AccountTenantId
          };

          var response = await eventBus.RequestAsync<AccountTenantActionMsg>(requestedMessage);
          if (!string.IsNullOrEmpty(response.ErrorMsg))
          {
            throw new Exception(response.ErrorMsg);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }

    }

    public async Task RequestResetTenantAdminPassword(AccountValueObject accountValueObject, Guid jobId)
    {
      try
      {
        using (CurrentTenant.Change(null))
        {
          var requestedMessage = new ResetTenantAdminPasswordMsg()
          {
            ObjectType = "Account",
            DocumentId = jobId.ToString()
          };
          requestedMessage.AccountXml = xmlSerializerHelper.SerializeToXml(accountValueObject);
          requestedMessage.DocumentId = accountValueObject.Id.ToString();

          var response = await eventBus.RequestAsync<AccountTenantActionMsg>(requestedMessage);
          if (!string.IsNullOrEmpty(response.ErrorMsg))
          {
            throw new Exception(response.ErrorMsg);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }

    }

    public async Task<Guid?> RequestCreateTenant(AccountValueObject accountValueObject, Guid jobId)
    {
      Guid? result = Guid.Empty;
      try
      {
        using (CurrentTenant.Change(null))
        {
          var requestedMessage = new CreateTenantFromAccountMsg
          {
            ObjectType = "Account",
            AdminEmail = accountValueObject.AdminEmail,
            AdminPassword = accountValueObject.AdminPassword,
            TenantName = accountValueObject.AccountNameEng,
            NewDatabaseName = accountValueObject.NewDatabaseName,
            NewUserName = accountValueObject.NewUserName,
            NewUserPassword = accountValueObject.NewUserPassword,
            DefaultConnectionString = string.Empty,
            CreateDatabase = true,
            DocumentId = accountValueObject.Id.ToString()
          };

          var response = await eventBus.RequestAsync<SendNewAccountTenantMsg>(requestedMessage);
          result = response.NewAccountTenantId;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }

      return result;
    }

    private string GeneratePassword()
    {
      string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      string lowerChars = "abcdefghijklmnopqrstuvwxyz";
      string digitChars = "0123456789";
      string specialChars = "!@#$%^&*()_+";
      string charset = upperChars + lowerChars + digitChars + specialChars;
      int passwordLength = 10;
      Random random = new Random();
      string password = "";

      for (int i = 0; i < passwordLength; i++)
      {
        int randomIndex = random.Next(0, charset.Length);
        password += charset[randomIndex];
      }

      return password;
    }
  }
}
