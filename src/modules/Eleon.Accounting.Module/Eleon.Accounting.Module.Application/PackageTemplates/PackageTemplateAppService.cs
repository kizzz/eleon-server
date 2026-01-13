using Common.Module.Extensions;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.DomainServices;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Permissions;

namespace VPortal.Accounting.Module.PackageTemplates
{
  [Authorize(AccountingPermissions.General)]
  public class PackageTemplateAppService : ModuleAppService, IPackageTemplateAppService
  {
    private readonly IVportalLogger<PackageTemplateAppService> logger;
    private readonly PackageTemplateDomainService domainService;

    public PackageTemplateAppService(
        IVportalLogger<PackageTemplateAppService> logger,
        PackageTemplateDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<PagedResultDto<PackageTemplateDto>> GetPackageTemplateList(PackageTemplateListRequestDto input)
    {
      PagedResultDto<PackageTemplateDto> result = null;
      try
      {
        var pair = await domainService
            .GetPackageTemplatesList(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.SearchQuery,
                input.DateFilterStart,
                input.DateFilterEnd,
                input.BillingPeriodTypeFilter);
        var dtos = ObjectMapper
            .Map<List<PackageTemplateEntity>, List<PackageTemplateDto>>(pair.Value);
        result = new PagedResultDto<PackageTemplateDto>(pair.Key, dtos);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<string> RemovePackageTemplate(Guid id)
    {
      string result = string.Empty;
      try
      {
        result = await domainService.RemovePackageTemplate(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<PackageTemplateDto> GetPackageTemplateById(Guid id)
    {
      PackageTemplateDto result = null;
      try
      {
        var entity = await domainService.GetPackageTemplateById(id);
        result = ObjectMapper.Map<PackageTemplateEntity, PackageTemplateDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<PackageTemplateDto> CreatePackageTemplate(PackageTemplateDto updatedDto)
    {
      PackageTemplateDto result = null;
      logger.LogStart();
      try
      {
        if (updatedDto.PackageTemplateModules != null && updatedDto.PackageTemplateModules.Count > 0)
        {
          updatedDto.PackageTemplateModules.ForEach(x =>
          {
            if (x.Id == Guid.Empty || x.Id.IsTempGuid())
            {
              x.Id = Guid.NewGuid();
            }
          });
        }
        ;
        var mappedEntity = ObjectMapper.Map<PackageTemplateDto, PackageTemplateEntity>(updatedDto);
        var entity = await domainService.CreatePackageTemplate(mappedEntity);
        result = ObjectMapper.Map<PackageTemplateEntity, PackageTemplateDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      logger.LogFinish();
      return result;
    }
    public async Task<PackageTemplateDto> UpdatePackageTemplate(PackageTemplateDto updatedDto)
    {
      PackageTemplateDto result = null;
      try
      {
        if (updatedDto.PackageTemplateModules != null && updatedDto.PackageTemplateModules.Count > 0)
        {
          updatedDto.PackageTemplateModules.ForEach(x =>
          {
            if (x.Id == Guid.Empty || x.Id.IsTempGuid())
            {
              x.Id = Guid.NewGuid();
            }
          });
        }
        ;

        var mappedEntity = ObjectMapper.Map<PackageTemplateDto, PackageTemplateEntity>(updatedDto);
        var entity = await domainService.UpdatePackageTemplate(mappedEntity);
        result = ObjectMapper.Map<PackageTemplateEntity, PackageTemplateDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
