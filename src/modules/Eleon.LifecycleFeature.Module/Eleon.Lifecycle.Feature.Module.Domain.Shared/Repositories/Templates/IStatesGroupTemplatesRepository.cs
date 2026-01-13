using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.Repositories.Templates
{
  public interface IStatesGroupTemplatesRepository : IBasicRepository<StatesGroupTemplateEntity, Guid>
  {
    public Task<List<StatesGroupTemplateEntity>> GetListAsync(
        string documentObjectType,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0);
    public Task<bool> Add(StatesGroupTemplateEntity statesGroupTemplate);
    Task<int> GetDocumentTypeGroupsCountAsync(string documentObjectType);
    Task<StateActorTemplateEntity> GetStateActor(Guid id);
    Task<StateTemplateEntity> GetState(Guid id);

    Task<KeyValuePair<long, List<StatesGroupTemplateEntity>>> GetPaginatedListAsync(
        string documentObjectType,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0);
  }
}
