using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.EntityFrameworkCore;

namespace VPortal.Auditor.Module.Repositories
{
  public class AuditHistoryRecordRepository :
      EfCoreRepository<AuditorDbContext, AuditHistoryRecordEntity, Guid>,
      IAuditHistoryRecordRepository
  {
    private readonly IVportalLogger<AuditHistoryRecordRepository> logger;
    public AuditHistoryRecordRepository(
        IDbContextProvider<AuditorDbContext> dbContextProvider,
        IVportalLogger<AuditHistoryRecordRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<AuditHistoryRecordEntity> GetRecordByDocumentVersion(string documentObjectType, string documentId, string version)
    {
      AuditHistoryRecordEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.AuditHistoryRecords
            .FirstOrDefaultAsync(x => x.DocumentObjectType == documentObjectType && x.DocumentId == documentId && x.Version == version);
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

    public async Task<List<AuditHistoryRecordEntity>> GetRecordsByVersion(Guid versionId, string version)
    {
      List<AuditHistoryRecordEntity> result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        var filtered = dbContext.AuditHistoryRecords
            .Where(x => x.AuditVersionId == versionId && x.Version == version);
        result = await filtered.ToListAsync();
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

    public async Task<KeyValuePair<int, List<AuditHistoryRecordEntity>>> GetRecordsByDocument(
        string documentObjectType,
        string documentId,
        string sorting,
        int maxResultCount,
        int skipCount,
        DateTime? fromDateFilter,
        DateTime? toDateFilter)
    {
      KeyValuePair<int, List<AuditHistoryRecordEntity>> result = new(0, null);
      try
      {
        var dbContext = await GetDbContextAsync();
        var filtered = dbContext.AuditHistoryRecords
            .Where(x => x.DocumentObjectType == documentObjectType && x.DocumentId == documentId)
            .WhereIf(fromDateFilter != null, x => x.CreationTime >= fromDateFilter)
            .WhereIf(toDateFilter != null, x => x.CreationTime <= fromDateFilter)
            .GroupBy(x => x.TransactionId)
            .Select(g => new
            {
              Items = g.OrderByDescending(x => x.CreationTime).Take(1),
            });
        var filteredEntities = (await filtered.ToListAsync()).SelectMany(x => x.Items);

        var paginated = filteredEntities
           .OrderByDescending(x => x.CreationTime)
           .Skip(skipCount)
           .Take(maxResultCount)
           .ToList();

        var count = filteredEntities.Count();
        result = new(count, paginated);
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
