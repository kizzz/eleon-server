//using Common.EventBus.Module;
//using Common.Module.Constants;
//using Common.Module.Helpers;
//using Logging.Module;
//using Messaging.Module.Messages;
//using System;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Domain.Services;
//using Volo.Abp.EventBus.Distributed;
//using Volo.Abp.Identity;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.ObjectMapping;
//using Volo.Abp.Uow;
//using VPortal.Accounting.Module.AuditEntities;
//using VPortal.Accounting.Module.Entities;
//using VPortal.Infrastructure.Module.Entities;

//namespace VPortal.Accounting.Module.DomainServices
//{

//  public class AccountAuditManager : DomainService
//  {
//    private readonly IVportalLogger<AccountAuditManager> logger;
//    private readonly IdentityUserManager userManager;
//    private readonly IObjectMapper objectMapper;
//    private readonly XmlSerializerHelper _xmlSerializerHelper;
//    private readonly ICurrentTenant currentTenant;
//    private readonly IDistributedEventBus requestClient;

//    public AccountAuditManager(
//       IVportalLogger<AccountAuditManager> logger,
//       IdentityUserManager userManager,
//       IObjectMapper objectMapper,
//       XmlSerializerHelper xmlSerializerHelper,
//       ICurrentTenant currentTenant,
//       IDistributedEventBus requestClient)
//    {
//      this.logger = logger;
//      this.userManager = userManager;
//      this.objectMapper = objectMapper;
//      _xmlSerializerHelper = xmlSerializerHelper;
//      this.currentTenant = currentTenant;
//      this.requestClient = requestClient;
//    }

//    public async Task<bool> CreateAccountAudit(AccountEntity account, DocumentVersionEntity knownVersion)
//    {
//      bool result = false;
//      try
//      {
//        using (currentTenant.Change(null))
//        {
//          var accountAuditEntity = objectMapper.Map<AccountEntity, AccountAuditEntity>(account);
//          var accountXml = _xmlSerializerHelper.SerializeToXml(accountAuditEntity);
//          string id = account.Id.ToString();

//          var request = new CreateAuditMsg()
//          {
//            AuditedDocumentId = id,
//            AuditedDocumentObjectType = "Account",
//            DocumentVersion = knownVersion,
//            RefDocumentId = id,
//            RefDocumentObjectType = "Account",
//            DocumentData = accountXml,
//          };

//          var response = await requestClient.RequestAsync<AuditCreatedMsg>(request);
//          result = response.CreatedSuccessfully;
//        }
//      }
//      catch (Exception e)
//      {
//        logger.Capture(e);
//      }

//      return result;
//    }

//    public async Task<DocumentVersionEntity> IncrementAccountVersion(Guid accountId, DocumentVersionEntity knownVersion)
//    {
//      DocumentVersionEntity result = null;
//      try
//      {
//        using (currentTenant.Change(null))
//        {
//          var request = new IncrementAuditDocumentVersionMsg()
//          {
//            AuditedDocumentId = accountId.ToString(),
//            AuditedDocumentObjectType = "Account",
//            Version = knownVersion,
//          };

//          var response = await requestClient.RequestAsync<AuditVersionIncrementedMsg>(request);

//          if (!response.Success)
//          {
//            if (response.NewVersion?.CreatedByUserId != null && response.NewVersion?.CreatedAt != null)
//            {
//              var changedByUser = await userManager.GetByIdAsync((Guid)response.NewVersion.CreatedByUserId);
//              throw new BusinessException(AccountingErrorCodes.DocumentVersionIsOutdated)
//                  .WithData("ChangedBy", $"{changedByUser.Name} {changedByUser.Surname}")
//                  .WithData("ChangedAt", response.NewVersion.CreatedAt.ToString());
//            }
//            else
//            {
//              throw new Exception($"An error occured while creating audit for the Account {accountId}.");
//            }
//          }

//          result = response.NewVersion;
//        }
//      }
//      catch (BusinessException)
//      {
//        throw;
//      }
//      catch (Exception e)
//      {
//        logger.Capture(e);
//      }

//      return result;
//    }

//    public async Task<AccountAuditEntity> GetAccountAudit(Guid accountId, string version)
//    {
//      AccountAuditEntity result = null;
//      try
//      {
//        using (currentTenant.Change(null))
//        {
//          var request = new GetAuditDocumentMsg()
//          {
//            AuditedDocumentId = accountId.ToString(),
//            AuditedDocumentObjectType = "Account",
//            Version = version,
//          };

//          var response = await requestClient.RequestAsync<AuditDocumentGotMsg>(request);

//          var entity = _xmlSerializerHelper.DeserializeFromXml<AccountAuditEntity>(response.AuditedDocument.Data);
//          entity.DocumentVersionEntity = response.AuditedDocument.Version;
//          result = entity;
//        }
//      }
//      catch (Exception e)
//      {
//        logger.Capture(e);
//      }

//      return result;
//    }

//    public async Task<DocumentVersionEntity> GetCurrentAccountVersion(Guid accountId)
//    {
//      DocumentVersionEntity result = null;
//      try
//      {
//        using (currentTenant.Change(null))
//        {
//          var request = new GetAuditCurrentVersionMsg()
//          {
//            RefDocumentId = accountId.ToString(),
//            RefDocumentObjectType = "Account",
//          };

//          var response = await requestClient.RequestAsync<AuditCurrentVersionGotMsg>(request);

//          result = response.CurrentVersion;
//        }
//      }
//      catch (Exception e)
//      {
//        logger.Capture(e);
//      }

//      return result;
//    }
//  }
//}
