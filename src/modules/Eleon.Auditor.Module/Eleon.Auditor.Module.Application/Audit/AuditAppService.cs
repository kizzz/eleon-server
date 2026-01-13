using Logging.Module;
using ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using VPortal.Auditor.Module.Application.Contracts.Audit;
using VPortal.Auditor.Module.DomainServices;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.Application.Audit;
public class AuditAppService : ModuleAppService, IAuditAppService
{
  private readonly IVportalLogger<AuditAppService> logger;
  private readonly AuditDomainService auditDomainService;

  public AuditAppService(
      IVportalLogger<AuditAppService> logger,
      AuditDomainService auditDomainService
      )
  {
    this.logger = logger;
    this.auditDomainService = auditDomainService;
  }

  public async Task<bool> CreateAsync(CreateAuditDto input)
  {

    var result = false;
    try
    {
      result = await auditDomainService.CreateAudit(
              input.RefDocumentObjectType,
              input.RefDocumentId,
              input.AuditedDocumentObjectType,
              input.AuditedDocumentId,
              input.DocumentData,
              input.DocumentVersion);
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
  public async Task<AuditDto> GetAsync(GetAuditDto input)
  {

    AuditDto result = null;
    try
    {
      var entity = await auditDomainService.GetAuditDocument(
          input.AuditedDocumentObjectType,
          input.AuditedDocumentId,
          input.Version);
      result = new AuditDto(entity.data, entity.version);
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
  public async Task<DocumentVersionEntity> GetCurrentVersionAsync(GetVersionDto input)
  {

    DocumentVersionEntity result = null;
    try
    {
      result = await auditDomainService.GetCurrentVersion(input.RefDocumentObjectType, input.RefDocumentId);
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

  public async Task<IncrementVersionResultDto> IncrementAuditVersionAsync(IncrementVersionRequestDto input)
  {

    IncrementVersionResultDto result = null;
    try
    {
      var (success, newVersion) = await auditDomainService.IncrementAuditVersion(
          input.AuditedDocumentObjectType,
          input.AuditedDocumentId,
          input.Version);
      result = new IncrementVersionResultDto
      {
        Success = success,
        NewDocumentVersion = newVersion
      };
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
}
