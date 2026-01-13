using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  [Authorize(LifecyclePermissions.LifecycleManager)]
  public class StatesGroupAuditAppService : ModuleAppService, IStatesGroupAuditAppService
  {
    private readonly IVportalLogger<StatesGroupAuditAppService> logger;
    private readonly StatesGroupAuditDomainService statesGroupAuditDomain;

    public StatesGroupAuditAppService(
        IVportalLogger<StatesGroupAuditAppService> logger,
        StatesGroupAuditDomainService statesGroupAuditDomain)
    {
      this.logger = logger;
      this.statesGroupAuditDomain = statesGroupAuditDomain;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StatesGroupAuditDto statesGroupAuditDto)
    {
      bool result = false;
      try
      {
        var statesGroupAudit = ObjectMapper
            .Map<StatesGroupAuditDto, StatesGroupAuditEntity>(statesGroupAuditDto);
        result = await statesGroupAuditDomain.Add(statesGroupAudit);
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

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        result = await statesGroupAuditDomain.Remove(id);
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

    public async Task<bool> DeepCancel(string docType, string documentId)
    {
      bool result = false;
      try
      {
        result = await statesGroupAuditDomain.DeepCancel(docType, documentId);
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

    public async Task<PagedResultDto<StatesGroupAuditReportDto>> GetReports(PendingApprovalRequestDto input)
    {
      try
      {
        var keyValue = await statesGroupAuditDomain.GetReports(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.SearchQuery,
            input.StatusDateFilterStart,
            input.StatusDateFilterEnd,
            input.ObjectTypeFilter,
            input.UserId,
            input.StatesGroupTemplateId);

        var statesGroupAuditList = keyValue.Value;

        var mapped = ObjectMapper
            .Map<List<StatesGroupAuditEntity>, List<StatesGroupAuditReportDto>>(statesGroupAuditList);

        return new PagedResultDto<StatesGroupAuditReportDto>(keyValue.Key, mapped);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return null;
    }

    public async Task<StatesGroupAuditDto> GetById(Guid id)
    {

      try
      {
        var entity = await statesGroupAuditDomain.GetById(id);
        return ObjectMapper.Map<StatesGroupAuditEntity, StatesGroupAuditDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
