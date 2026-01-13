using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.Repositories;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.DomainServices
{

  public class AuditDomainService : DomainService
  {
    private readonly IVportalLogger<AuditDomainService> logger;
    private readonly IAuditHistoryRecordRepository historyRecordRepository;
    private readonly IAuditDataRepository dataRepository;
    private readonly IAuditCurrentVersionRepository currentVersionRepository;
    private readonly ICurrentUser currentUser;
    private readonly IDistributedEventBus _eventBus;

    public AuditDomainService(
        IVportalLogger<AuditDomainService> logger,
        IAuditHistoryRecordRepository historyRecordRepository,
        IAuditDataRepository dataRepository,
        IAuditCurrentVersionRepository currentVersionRepository,
        ICurrentUser currentUser,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.historyRecordRepository = historyRecordRepository;
      this.dataRepository = dataRepository;
      this.currentVersionRepository = currentVersionRepository;
      this.currentUser = currentUser;
      this._eventBus = eventBus;
    }

    /// <summary>
    /// Gets the current version of a given document.
    /// </summary>
    /// <param name="refDocumentObjectType">The reference document's object type.</param>
    /// <param name="refDocumentId">The reference document's ID.</param>
    /// <returns>The current version of the document.</returns>
    public async Task<DocumentVersionEntity> GetCurrentVersion(
        string refDocumentObjectType,
        string refDocumentId)
    {
      DocumentVersionEntity result = null;
      try
      {
        var currentVersion = await currentVersionRepository.GetCurrentVersion(refDocumentObjectType, refDocumentId);
        /*if (currentVersion == null)
        {
            string nextVersion = await seriesDomainService.GetNextSeriaNumber(
                DocumentObjectType.Audit,
                DocumentTypes.New,
                GetSeriaRefId(refDocumentObjectType, refDocumentId));
            currentVersion = new AuditCurrentVersionEntity(GuidGenerator.Create())
            {
                RefDocId = refDocumentId,
                RefDocumentType = refDocumentObjectType,
                CurrentVersion = nextVersion,
            };
            await currentVersionRepository.InsertAsync(currentVersion, true);
        }*/
        if (currentVersion != null)
        {
          result = new DocumentVersionEntity
          {
            Version = currentVersion.CurrentVersion,
            CreatedAt = currentVersion.LastModificationTime ?? currentVersion.CreationTime,
            CreatedByUserId = currentVersion.LastModifierId ?? currentVersion.CreatorId,
            CreatedByUserName = currentVersion.LastModifierName,
          };
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

    public async Task<(bool success, DocumentVersionEntity version)> IncrementAuditVersion(
        string refDocumentObjectType,
        string refDocumentId,
        DocumentVersionEntity claimedVersion)
    {
      (bool success, DocumentVersionEntity version) result = (false, null);
      AuditCurrentVersionEntity currentVersion = null;
      try
      {
        currentVersion = await currentVersionRepository.GetCurrentVersion(refDocumentObjectType, refDocumentId);
        string oldVersion = currentVersion?.CurrentVersion;

        if (claimedVersion == null || oldVersion == claimedVersion.Version)
        {
          var response = await _eventBus.RequestAsync<DocumentSeriaNumberGotMsg>(new GetDocumentSeriaNumberMsg { ObjectType = "Audit", Prefix = Prefixes.ObjectTypePrefixes["Audit"], RefId = GetSeriaRefId(refDocumentObjectType, refDocumentId) });
          var nextVersion = response.SeriaNumber;

          if (currentVersion == null)
          {
            currentVersion = new AuditCurrentVersionEntity(GuidGenerator.Create())
            {
              RefDocId = refDocumentId,
              RefDocumentType = refDocumentObjectType,
              CurrentVersion = nextVersion,
              LastModifierName = GetCurrentUserName(),
            };
            await currentVersionRepository.InsertAsync(currentVersion, true);
          }
          else
          {
            currentVersion.CurrentVersion = nextVersion;
            currentVersion.LastModifierName = GetCurrentUserName();
            await currentVersionRepository.UpdateAsync(currentVersion, true);
          }

          string transactionId = claimedVersion?.AppendToTransactionId ?? currentVersion.ConcurrencyStamp;
          if (oldVersion != null)
          {
            await CopyVersion(currentVersion.Id, oldVersion, currentVersion.CurrentVersion, transactionId);
          }

          var newVersion = new DocumentVersionEntity
          {
            Version = currentVersion.CurrentVersion,
            CreatedAt = currentVersion.LastModificationTime ?? currentVersion.CreationTime,
            CreatedByUserId = currentVersion.LastModifierId ?? currentVersion.CreatorId,
            CreatedByUserName = currentVersion.LastModifierName,
            TransactionId = transactionId,
          };

          var versionChangeMessage = new AuditVersionChangeNotificationMsg
          {
            AuditChange = new AuditVersionChangeNotificationEto()
            {
              DocumentId = refDocumentId,
              DocumentObjectType = refDocumentObjectType,
              NewVersion = newVersion,
            }
          };
          await _eventBus.PublishAsync(versionChangeMessage);
          //await auditorHubContext.NotifyVersionChanged(
          //    refDocumentObjectType,
          //    refDocumentId,
          //    newVersion);

          result = (true, newVersion);
        }
      }
      catch (Exception e)
      {
        result = (false, new DocumentVersionEntity
        {
          Version = currentVersion?.CurrentVersion,
          CreatedAt = currentVersion?.LastModificationTime ?? currentVersion.CreationTime,
          CreatedByUserId = currentVersion?.LastModifierId ?? currentVersion.CreatorId,
          CreatedByUserName = currentVersion.LastModifierName,
        });
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CreateAudit(
        string refDocumentObjectType,
        string refDocumentId,
        string documentObjectType,
        string documentId,
        string documentData,
        DocumentVersionEntity version)
    {
      bool result = false;
      try
      {
        var currentVersion = await currentVersionRepository.GetCurrentVersion(refDocumentObjectType, refDocumentId);

        var newData = new AuditDataEntity(GuidGenerator.Create())
        {
          DocumentData = documentData,
        };
        await dataRepository.InsertAsync(newData);

        var targetVersion = version?.Version ?? currentVersion?.CurrentVersion;
        var documentHistoryRecord = await historyRecordRepository.GetRecordByDocumentVersion(documentObjectType, documentId, targetVersion);
        if (documentHistoryRecord == null)
        {
          documentHistoryRecord = new AuditHistoryRecordEntity(GuidGenerator.Create())
          {
            AuditDataId = newData.Id,
            Version = targetVersion,
            DocumentId = documentId,
            DocumentObjectType = documentObjectType,
            AuditVersionId = currentVersion.Id,
            TransactionId = version?.TransactionId,
            CreatorName = GetCurrentUserName(),
          };
          await historyRecordRepository.InsertAsync(documentHistoryRecord, true);
        }
        else
        {
          documentHistoryRecord.AuditDataId = newData.Id;
          documentHistoryRecord.TransactionId = version?.TransactionId;
          await historyRecordRepository.UpdateAsync(documentHistoryRecord, true);
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

    public async Task<(DocumentVersionEntity version, string data)> GetAuditDocument(
        string documentObjectType,
        string documentId,
        string version)
    {
      (DocumentVersionEntity version, string data) result = (null, null);
      try
      {
        var auditDocument = await historyRecordRepository.GetRecordByDocumentVersion(documentObjectType, documentId, version);
        if (auditDocument != null)
        {
          var auditDocumentData = await dataRepository.FindAsync(auditDocument.AuditDataId);
          result = (new DocumentVersionEntity
          {
            CreatedAt = auditDocument.CreationTime,
            CreatedByUserId = auditDocument.CreatorId,
            CreatedByUserName = auditDocument.CreatorName,
            TransactionId = auditDocument.TransactionId,
            Version = version,
          }, auditDocumentData.DocumentData);
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

    public async Task<KeyValuePair<int, List<DocumentVersionEntity>>> GetAuditDocumentHistory(
        string documentObjectType,
        string documentId,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        DateTime? fromDateFilter = null,
        DateTime? toDateFilter = null)
    {
      KeyValuePair<int, List<DocumentVersionEntity>> result = new(0, null);
      try
      {
        var (count, history) = await historyRecordRepository.GetRecordsByDocument(
            documentObjectType,
            documentId,
            sorting,
            maxResultCount,
            skipCount,
            fromDateFilter,
            toDateFilter);

        var versions = history.Select(x => new DocumentVersionEntity
        {
          CreatedAt = x.CreationTime,
          CreatedByUserId = x.CreatorId,
          Version = x.Version,
          CreatedByUserName = x.CreatorName,
        }).ToList();
        result = new(count, versions);
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

    public async Task CopyVersion(
        Guid currentVersionId,
        string versionToCopy,
        string newVersion,
        string transactionId)
    {
      try
      {
        var oldVersionRecords = await historyRecordRepository.GetRecordsByVersion(currentVersionId, versionToCopy);

        string currentUserName = GetCurrentUserName();
        var copies = new List<AuditHistoryRecordEntity>();
        foreach (var toCopy in oldVersionRecords)
        {
          var copy = new AuditHistoryRecordEntity(GuidGenerator.Create())
          {
            AuditDataId = toCopy.AuditDataId,
            Version = newVersion,
            DocumentId = toCopy.DocumentId,
            DocumentObjectType = toCopy.DocumentObjectType,
            AuditVersionId = toCopy.AuditVersionId,
            CreatorName = currentUserName,
            TransactionId = transactionId,
          };
          copies.Add(copy);
        }

        await historyRecordRepository.InsertManyAsync(copies, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    private string GetCurrentUserName()
    {
      if (currentUser.Id == null)
      {
        return null;
      }

      string currentUserName = null;
      if (!currentUser.Name.IsNullOrEmpty())
      {
        currentUserName = currentUser.Name;
      }

      if (!currentUser.SurName.IsNullOrEmpty())
      {
        currentUserName = string.IsNullOrWhiteSpace(currentUserName)
            ? currentUser.SurName
            : currentUserName + " " + currentUser.SurName;
      }

      if (string.IsNullOrWhiteSpace(currentUserName))
      {
        currentUserName = currentUser.UserName;
      }

      return currentUserName;
    }

    private static string GetSeriaRefId(string documentObjectType, string refDocumentId)
        => $"{documentObjectType};{refDocumentId}";
  }
}
