using Common.Module.Constants;
using Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using ModuleCollector.LifecycleFeatureModule.Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.LifecycleManager
{
  public interface ILifecycleManagerAppService : IApplicationService
  {
    public Task<StatesGroupAuditTreeDto> GetTrace(string documentObjectType, string documentId);
    public Task<bool> CanApprove(string documentObjectType, string documentId);
    public Task<bool> CanReview(string documentObjectType, string documentId);
    public Task<bool> Approve(string documentObjectType, string documentId);
    public Task<bool> Reject(string documentObjectType, string documentId, string reason);
    public Task<LifecycleStatusDto> GetLifecycleStatus(string documentObjectType, string documentId);
    Task<bool> GetViewPermissionAsync(Guid initiatorId, string documentObjectType, string documentId, bool review = true);
    Task<List<string>> GetDocumentIdsByFilterAsync(GetDocumentIdsByFilterRequestDto input);
    Task<StatesGroupAuditTreeDto> StartExistingLifecycleAsync(string documentObjectType, string documentId);
    Task<StatesGroupAuditTreeDto> StartNewLifecycleAsync(StartNewLifecycleRequestDto input);
  }
}
