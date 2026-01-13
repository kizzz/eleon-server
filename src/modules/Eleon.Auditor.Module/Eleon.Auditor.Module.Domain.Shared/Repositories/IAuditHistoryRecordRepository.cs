using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Auditor.Module.Entities;

namespace VPortal.Auditor.Module.Repositories
{
  public interface IAuditHistoryRecordRepository : IBasicRepository<AuditHistoryRecordEntity, Guid>
  {
    Task<List<AuditHistoryRecordEntity>> GetRecordsByVersion(Guid versionId, string version);
    Task<KeyValuePair<int, List<AuditHistoryRecordEntity>>> GetRecordsByDocument(
        string documentObjectType,
        string documentId,
        string sorting,
        int maxResultCount,
        int skipCount,
        DateTime? fromDateFilter,
        DateTime? toDateFilter);
    Task<AuditHistoryRecordEntity> GetRecordByDocumentVersion(string documentObjectType, string documentId, string version);
  }
}
