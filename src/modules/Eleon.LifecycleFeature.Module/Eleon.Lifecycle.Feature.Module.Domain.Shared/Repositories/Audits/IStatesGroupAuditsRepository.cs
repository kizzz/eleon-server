using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Audits
{
  public interface IStatesGroupAuditsRepository : IBasicRepository<StatesGroupAuditEntity, Guid>
  {
    public Task<bool> Add(StatesGroupAuditEntity statesGroupAudit);
    public Task<bool> Remove(Guid id);
    public Task<StatesGroupAuditEntity> GetByDocIdAsync(string documentObjectType, string documentId);
    Task<StateActorAuditEntity> GetStateActor(Guid id);
    Task<StateAuditEntity> GetState(Guid id);
    Task<KeyValuePair<long, List<StatesGroupAuditEntity>>> GetReportListsAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? statusDateFilterStart = null,
        DateTime? statusDateFilterEnd = null,
        IList<string>? objectTypeFilter = null,
        Guid? userId = null,
        IList<string>? roles = null,
        Guid? statesTemplateGroupId = null);
    Task<List<string>> GetDocumentIdsByFilter(string documentObjectType, Guid? userId = null, List<string> roles = null, List<LifecycleStatus> lifecycleStatuses = null);
  }
}
