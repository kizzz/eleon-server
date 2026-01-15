using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using Common.Module.ValueObjects;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client;
using Migrations.Module;
using ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Accounting.Module.AuditEntities;
using VPortal.Accounting.Module.Constants;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Localization;
using VPortal.Accounting.Module.Repositories;
using VPortal.Accounting.Module.Tenant;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.DomainServices
{

  public class AccountDomainService : DomainService
  {
    private readonly IVportalLogger<AccountDomainService> logger;
    private readonly IObjectMapper objectMapper;
    private readonly IAccountRepository accountRepository;
    private readonly IdentityUserManager userManager;
    private readonly IDistributedEventBus requestClient;
    private readonly CurrentUser currentUser;
    private readonly BillingInformationDomainService billingInformationDomainService;
    private readonly IDistributedEventBus attachConfiguredUserFieldSetsClient;
    private readonly IPermissionChecker permissionChecker;
    private readonly ICurrentTenant currentTenant;
    private readonly AccountTenantManager accountTenantManager;
    private readonly IDistributedEventBus messagePublisher;
    private readonly IStringLocalizer<AccountingResource> localizer;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IPackageTemplateRepository packageTemplateRepository;

    public AccountDomainService(IVportalLogger<AccountDomainService> logger,
        IObjectMapper objectMapper,
        IAccountRepository accountRepository,
        IdentityUserManager userManager,
        IDistributedEventBus requestClient,
        CurrentUser currentUser,
        BillingInformationDomainService billingInformationDomainService,
        IDistributedEventBus attachConfiguredUserFieldSetsClient,
        IPermissionChecker permissionChecker,
        ICurrentTenant currentTenant,
        AccountTenantManager accountTenantManager,
        IDistributedEventBus messagePublisher,
        IStringLocalizer<AccountingResource> localizer,
        IUnitOfWorkManager unitOfWorkManager,
        IPackageTemplateRepository packageTemplateRepository)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.accountRepository = accountRepository;
      this.userManager = userManager;
      this.requestClient = requestClient;
      this.currentUser = currentUser;
      this.billingInformationDomainService = billingInformationDomainService;
      this.attachConfiguredUserFieldSetsClient = attachConfiguredUserFieldSetsClient;
      this.permissionChecker = permissionChecker;
      this.currentTenant = currentTenant;
      this.accountTenantManager = accountTenantManager;
      this.messagePublisher = messagePublisher;
      this.localizer = localizer;
      this.unitOfWorkManager = unitOfWorkManager;
      this.packageTemplateRepository = packageTemplateRepository;
    }

    public async Task<AccountEntity> GetAccountDetailsById(Guid id)
    {
      AccountEntity result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          result = await accountRepository.GetAsync(id, true);

          foreach (var package in result.AccountPackages)
          {
            var templatePackage = await packageTemplateRepository.GetAsync(package.PackageTemplateEntityId);
            package.Name = templatePackage.PackageName;
          }
          //if (requiredVersion == null)
          //{
          //  // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
          //  // var version = await auditManager.GetCurrentAccountVersion(id);
          //  var entity = await accountRepository.GetAsync(id, true);
          //  // entity.DocumentVersionEntity = version;
          //  result = entity;
          //}
          //else
          //{
          //  // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
          //  // var auditEntity = await auditManager.GetAccountAudit(id, requiredVersion);
          //  // result = objectMapper.Map<AccountAuditEntity, AccountEntity>(auditEntity);
          //  result = await accountRepository.GetAsync(id, true);
          //}
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<AccountEntity> CreateAccount(
        string dataSourceUid,
        string dataSourceName,
        string companyUid,
        string companyName,
        Guid organizationUnitId,
        string organizationUnitName,
        Guid ownerId
      )
    {
      AccountEntity result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          Guid id = GuidGenerator.Create();
          // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
          // var newVersion = await auditManager.IncrementAccountVersion(id, null);

          var toAdd = new AccountEntity(id)
          {
            DataSourceUid = dataSourceUid,
            DataSourceName = dataSourceName,
            CompanyUid = companyUid,
            CompanyName = companyName,
            OrganizationUnitId = organizationUnitId,
            OrganizationUnitName = organizationUnitName,
            AccountStatus = AccountStatus.New,
            OwnerId = ownerId

          };

          result = await accountRepository.InsertAsync(toAdd, true);
          if (result == null)
          {
            throw new Exception("Unable to insert a new Account.");
          }

          // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
          // await auditManager.CreateAccountAudit(added, newVersion);
          // added.DocumentVersionEntity = newVersion;
        }

        // ❌ DEPRECATED: Changed return type from AccountAuditEntity to AccountEntity
        // result = objectMapper.Map<AccountEntity, AccountAuditEntity>(added);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<AccountEntity> UpdateAccount(AccountEntity updatedEntity)
    {
      AccountEntity result = null;
      try
      {
        var currentTenantId = currentTenant.Id;

        using (currentTenant.Change(null))
        {
          var isTenantExist = currentTenant.Id.HasValue;
          var existingEntity = await accountRepository.GetAsync(updatedEntity.Id, true);
          if (existingEntity.CreatorId != currentUser.Id)
          {
            throw new AbpAuthorizationException();
          }

          var isAccountManager = await IsAccountManager();

          if (isAccountManager && existingEntity.AccountStatus == AccountStatus.New && updatedEntity.AccountStatus == AccountStatus.Active)
          {
            existingEntity.AccountStatus = AccountStatus.Active;
          }

          // ❌ DEPRECATED
          // var newVersion = await auditManager.IncrementAccountVersion(updatedEntity.Id, updatedEntity.DocumentVersionEntity);

          existingEntity.CurrentBalance = updatedEntity.CurrentBalance;
          existingEntity.AccountName = updatedEntity.AccountName;

          existingEntity.OwnerId = updatedEntity.OwnerId;
          existingEntity.Members = updatedEntity.Members;

          existingEntity.BillingInformation.CompanyName = updatedEntity.BillingInformation.CompanyName;
          existingEntity.BillingInformation.CompanyCID = updatedEntity.BillingInformation.CompanyCID;
          existingEntity.BillingInformation.BillingAddressLine1 = updatedEntity.BillingInformation.BillingAddressLine1;
          existingEntity.BillingInformation.BillingAddressLine2 = updatedEntity.BillingInformation.BillingAddressLine2;
          existingEntity.BillingInformation.City = updatedEntity.BillingInformation.City;
          existingEntity.BillingInformation.StateOrProvince = updatedEntity.BillingInformation.StateOrProvince;
          existingEntity.BillingInformation.PostalCode = updatedEntity.BillingInformation.PostalCode;
          existingEntity.BillingInformation.Country = updatedEntity.BillingInformation.Country;
          existingEntity.BillingInformation.ContactPersonName = updatedEntity.BillingInformation.ContactPersonName;
          existingEntity.BillingInformation.ContactPersonEmail = updatedEntity.BillingInformation.ContactPersonEmail;
          existingEntity.BillingInformation.ContactPersonTelephone = updatedEntity.BillingInformation.ContactPersonTelephone;
          existingEntity.BillingInformation.PaymentMethod = updatedEntity.BillingInformation.PaymentMethod;

          existingEntity.AccountPackages.Difference(updatedEntity.AccountPackages, x => x.Id).Apply((original, changed) =>
          {
            original.TenantId = null;
            original.AutoSuspention = changed.AutoSuspention;
            original.ExpiringDate = changed.ExpiringDate;
            original.PackageTemplateEntityId = changed.PackageTemplateEntityId;
            original.LastBillingDate = DateTime.Now;
            original.NextBillingDate = SetNextBillingDate(changed.BillingPeriodType, changed.LastBillingDate);
            original.BillingPeriodType = changed.BillingPeriodType;
            original.OneTimeDiscount = changed.OneTimeDiscount;
            original.PermanentDiscount = changed.PermanentDiscount;
            original.AutoRenewal = changed.AutoRenewal;
            original.Status = AccountStatus.New;
            original.LinkedMembers = changed.LinkedMembers;
          });

          var entity = await accountRepository.UpdateAsync(existingEntity, true);
          // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
          // await auditManager.CreateAccountAudit(existingEntity, newVersion);
          // entity.DocumentVersionEntity = newVersion;


          // ❌ DEPRECATED: Changed return type from DocumentVersionEntity to AccountEntity
          // ObjectMapper.Map<AccountEntity, AccountAuditEntity>(entity);
          result = entity;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task CreateTenantJobSucced(Guid id, Guid accountTenantId)
    {
      try
      {
        using (currentTenant.Change(null))
        {
          AccountEntity accountEntity = await accountRepository.GetAsync(id, true);
          accountEntity.AccountStatus = AccountStatus.Active;
          accountEntity.TenantId = accountTenantId;

          var result = await accountRepository.UpdateAsync(accountEntity, true);

          await ResendAccountInfo(accountEntity.Id);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task CreateTenantJobFailed(Guid id)
    {
      try
      {
        using (currentTenant.Change(null))
        {
          AccountEntity accountEntity = await accountRepository.GetAsync(id);
          accountEntity.AccountStatus = AccountStatus.Generating;
          await accountRepository.UpdateAsync(accountEntity, true);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task<string> CancelAccount(Guid id)
    {
      string result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          // Audit: AccountEntity accountEntity = await PrepareAccountForJob(id);
          result = await accountTenantManager.RequestAccountTenantActionJobExecution(id, AccountingBackgroundJobTypes.CancelAccountTenant);
        }
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }

      return result;
    }

    public async Task<string> ResendAccountInfo(Guid id)
    {
      string result = null;
      try
      {
        using (currentTenant.Change(null))
        {
          AccountEntity accountEntity = await accountRepository.GetAsync(id);
          var notificationGuid = Guid.NewGuid();
          var notificationMessage = new NotificatorRequestedMsg();

          var owner = await userManager.FindByIdAsync(accountEntity.OwnerId.ToString());
          if (owner == null)
          {
            throw new Exception("Owner not found");
          }
          notificationMessage.Notification = new EleonsoftNotification()
          {
            Id = notificationGuid,
            Message = string.Join(",", owner.UserName,
                                  owner.Email),
            Type = new MessageNotificationType { TemplateName = AccountingTemplateConsts.ResendAccountInfoTemplate, LanguageKeyParams = [] },
            Recipients = new List<RecipientEto>()
                        {
                            new RecipientEto()
                            {
                                Type = NotificatorRecepientType.Direct,
                                RecipientAddress = owner.Email,
                            },
                        },
            RunImmidiate = true,
          };

          await messagePublisher.PublishAsync(notificationMessage);
        }
      }
      catch (Exception ex)
      {
        result = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> ActivateAccount(Guid id)
    {
      string result = null;
      try
      {
        // Audit: AccountEntity accountEntity = await PrepareAccountForJob(id);
        result = await accountTenantManager.RequestAccountTenantActionJobExecution(id, AccountingBackgroundJobTypes.ActivateAccountTenant);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> ResetAccount(Guid id)
    {
      string result = null;
      try
      {
        // Audit: AccountEntity accountEntity = await PrepareAccountForJob(id);
        result = await accountTenantManager.RequestResetAdminPasswordAccountTenantJobExecution(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> SuspendAccount(Guid id)
    {
      string result = null;
      try
      {
        // Audit: AccountEntity accountEntity = await PrepareAccountForJob(id);
        result = await accountTenantManager.RequestAccountTenantActionJobExecution(id, AccountingBackgroundJobTypes.SuspendAccountTenant);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
    public async Task ExecuteCancelAccount(Guid id)
    {
      try
      {

        using (currentTenant.Change(null))
        {
          var entity = await accountRepository.FindAsync(id);
          entity.AccountStatus = AccountStatus.Canceled;

          await accountRepository.UpdateAsync(entity);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task ExecuteSuspendAccount(Guid id)
    {
      try
      {

        using (currentTenant.Change(null))
        {
          AccountEntity accountEntity = await accountRepository.GetAsync(id);
          accountEntity.AccountStatus = AccountStatus.Suspended;
          await accountRepository.UpdateAsync(accountEntity, true);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task ExecuteActivateAccount(Guid id)
    {
      try
      {
        using (currentTenant.Change(null))
        {
          AccountEntity accountEntity = await accountRepository.GetAsync(id);
          accountEntity.AccountStatus = AccountStatus.Active;
          await accountRepository.UpdateAsync(accountEntity, true);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    [UnitOfWork(IsDisabled = true)]
    public async Task<KeyValuePair<long, List<AccountEntity>>> GetByFilter(
        AccountListRequestType requestType,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? creationDateFilterStart = null,
        DateTime? creationDateFilterEnd = null,
        string initiatorNameFilter = null,
        IList<AccountStatus> accountStatusFilter = null,
        IList<Guid> organizationUnitFilter = null
      )
    {
      KeyValuePair<long, List<AccountEntity>> result = new();
      try
      {
        var isAccountManager = await IsAccountManager();
        var user = await userManager.GetByIdAsync((Guid)currentUser.Id);
        if (!currentTenant.Id.HasValue && requestType != AccountListRequestType.Reseller && requestType != AccountListRequestType.ResellerByUser)
        {
          if (await permissionChecker.IsGrantedAsync("Permission.Account.AccountManager"))
          {
            requestType = requestType switch
            {
              AccountListRequestType.EnRoute => AccountListRequestType.EnRouteAccountManager,
              AccountListRequestType.ActionRequired => AccountListRequestType.ActionRequiredAccountManager,
              AccountListRequestType.Archive => AccountListRequestType.ArchiveAccountManager,
              _ => throw new NotImplementedException(),
            };
          }
        }
        List<string> documentIds = new();
        if (requestType == AccountListRequestType.ActionRequired || requestType == AccountListRequestType.ActionRequiredAccountManager)
        {
          var request = new GetDocumentIdsByFilterMsg
          {
            DocumentObjectType = "Account"
          };

          var roles = await userManager.GetRolesAsync(user);
          if (roles != null && roles.Count > 0)
          {
            request.Roles = roles.ToList();
          }
          request.UserId = user.Id;

          request.LifecycleStatuses = new List<LifecycleStatus>() { LifecycleStatus.Enroute };
          var response = await requestClient.RequestAsync<GetDocumentIdsByFilterGotMsg>(request);
          if (!response.IsSuccess)
          {
            throw new Exception(response.ErrorMsg);
          }

          documentIds = response.Ids;
        }

        using (currentTenant.Change(null))
        {
          using (var unitOfWork = unitOfWorkManager.Begin(true, false, System.Data.IsolationLevel.ReadUncommitted))
          {
            var accounts = await accountRepository.GetByFilter(
                requestType,
                user.Id,
                documentIds,
                sorting ?? "CreationTime desc",
                maxResultCount,
                skipCount,
                searchQuery,
                creationDateFilterStart,
                creationDateFilterEnd,
                initiatorNameFilter,
                accountStatusFilter,
                organizationUnitFilter);

            result = KeyValuePair.Create(accounts.Key, accounts.Value);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    private async Task<bool> IsAccountManager()
    {
      var result = false;
      try
      {
        Guid? userId = currentUser.Id;
        if (await permissionChecker.IsGrantedAsync("Permission.Account.AccountManager"))
        {
          result = true;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private DateTime SetNextBillingDate(BillingPeriodType billingPeriodType, DateTime lastBillingDate)
    {
      DateTime result = DateTime.Now;
      try
      {
        switch (billingPeriodType)
        {
          case BillingPeriodType.Month:
            result = lastBillingDate.AddMonths(1);
            break;
          case BillingPeriodType.Year:
            result = lastBillingDate.AddYears(1);
            break;
          case BillingPeriodType.Weekly:
            result = lastBillingDate.AddDays(7);
            break;
          case BillingPeriodType.Quarterly:
            result = lastBillingDate.AddMonths(3);
            break;
          case BillingPeriodType.None:
            result = DateTime.MaxValue;
            break;
          default:
            throw new Exception(string.Format($"Billing Period Type: {billingPeriodType} not recognized"));
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }


      return result;
    }

    //private async Task<AccountEntity> PrepareAccountForJob(Guid id)
    //{
    //  AccountEntity result = null;
    //  try
    //  {
    //    using (currentTenant.Change(null))
    //    {
    //      AccountEntity accountEntity = await GetAccountDetailsById(id, null);
    //      // ❌ DEPRECATED: DocumentVersionEntity related code has been commented
    //      // var newVersion = await auditManager.IncrementAccountVersion(accountEntity.Id, accountEntity.DocumentVersionEntity);
    //      // ❌ DEPRECATED: DocumentStatus, DocumentVersionEntity properties have been deleted
    //      // accountEntity.DocumentStatus = DocumentStatuses.Executing;
    //      result = await accountRepository.UpdateAsync(accountEntity, true);
    //      // await auditManager.CreateAccountAudit(accountEntity, newVersion);
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    logger.Capture(ex);
    //  }

    //  logger.LogFinish();

    //  return result;
    //}
  }
}
